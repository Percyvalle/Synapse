using System.Diagnostics;
using System.IO.Pipes;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Server;
using Serilog;
using Synapse.Common.Constants;
using Synapse.LSP.Abstractions.Interfaces;
using Synapse.LSP.Abstractions.Models;
using Synapse.LSP.Handlers;
using Synapse.LSP.Server.Handlers;
using ILogger = Serilog.ILogger;

namespace Synapse.LSP.Entities;

/// <summary>
/// Orchestrates the Language Server Protocol (LSP) lifecycle and request handling.
/// </summary>
public class LanguageServerHost : ILanguageServer
{
   private static readonly TimeSpan DefaultMonitorIntervalInternal = TimeSpan.FromMilliseconds(100);

   private readonly CancellationTokenSource _cancellation = new CancellationTokenSource();

   private readonly LanguageServerConfiguration _configuration;

   /// <summary>
   /// Initializes a new instance of the <see cref="LanguageServerHost"/> class.
   /// </summary>
   /// <param name="configuration">The configuration settings used to initialize the server.</param>
   /// <exception cref="ArgumentNullException">Thrown when <paramref name="configuration"/> is null.</exception>
   public LanguageServerHost(LanguageServerConfiguration configuration)
   {
      _configuration = configuration;
   }

   private ILogger Logger { get; } = Log.ForContext<LanguageServerHost>();

   /// <inheritdoc/>
   public async Task<int> RunAsync(CancellationToken token = default)
   {
      using var linked = CancellationTokenSource.CreateLinkedTokenSource(token, _cancellation.Token);
      using var stream = new NamedPipeServerStream(_configuration.PipeName, PipeDirection.InOut, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);

      Logger.Information("Listening for transport connection: {PipeName}", _configuration.PipeName);
      await stream.WaitForConnectionAsync(linked.Token).ConfigureAwait(false);

      Logger.Information("Transport connection established. Initializing Language Server...");
      var server = await InitializeServerAsync(stream).ConfigureAwait(false);

      Logger.Information("Language server initialization complete. Service is running.");
      await WaitForShutdownAsync(server, stream, linked.Token, token).ConfigureAwait(false);

      return ExitCodes.Success;
   }

   private async Task<LanguageServer> InitializeServerAsync(NamedPipeServerStream pipe)
   {
      return await LanguageServer.From(options =>
         options
            .WithInput(pipe)
            .WithOutput(pipe)
            .ConfigureLogging(
               builder => builder
                  .AddSerilog()
                  .AddLanguageProtocolLogging()
                  .SetMinimumLevel(LogLevel.Debug))
            .WithHandler<SynapseDocumentHandler>()
            .WithHandler<SynapseShutdownHandler>())
         .ConfigureAwait(false);
   }

   private async Task WaitForShutdownAsync(LanguageServer server, NamedPipeServerStream stream, CancellationToken linked, CancellationToken token)
   {
      // NOTE: We initiate background monitoring of the host process (IDE) to ensure
      // the server self-terminates if the IDE crashes unexpectedly without sending a formal 'shutdown' request.
      var monitor = Task.Run(() => MonitorHostProcessInternalAsync(server, linked), CancellationToken.None);

      var completed = await Task.WhenAny(server.WaitForExit, monitor).ConfigureAwait(false);
      if (completed == monitor && !token.IsCancellationRequested)
      {
         server.ForcefulShutdown();
         Logger.Warning("The host process was terminated unexpectedly. Stopping the language server.");
      }

      await _cancellation.CancelAsync().ConfigureAwait(false);

      if (stream.IsConnected)
      {
         await stream.DisposeAsync().ConfigureAwait(false);
      }

      try
      {
         await server.WaitForExit.ConfigureAwait(false);
      }
      catch (Exception exception) when (exception is OperationCanceledException or ObjectDisposedException)
      {
         Logger.Debug(exception, "Language server shutdown completed after transport termination.");
      }

      await monitor.ConfigureAwait(false);
   }

   private async Task MonitorHostProcessInternalAsync(LanguageServer server, CancellationToken token)
   {
      try
      {
         // Wait for the client to send the 'initialize' request and populate the ProcessId
         while (server.ClientSettings?.ProcessId == null && !token.IsCancellationRequested)
         {
            await Task.Delay(DefaultMonitorIntervalInternal, token);
         }

         if (token.IsCancellationRequested || server.ClientSettings?.ProcessId == null)
         {
            return;
         }

         var processId = (int)server.ClientSettings.ProcessId.Value;
         using (var process = Process.GetProcessById(processId))
         {
            await process.WaitForExitAsync(token).ConfigureAwait(false);

            if (!token.IsCancellationRequested)
            {
               await _cancellation.CancelAsync().ConfigureAwait(false);
            }
         }
      }
      catch (ArgumentException)
      {
         // The process with the specified ID has already exited.
         if (!token.IsCancellationRequested)
         {
            await _cancellation.CancelAsync().ConfigureAwait(false);
         }
      }
      catch (InvalidOperationException)
      {
         // The process has already exited or cannot be tracked.
         if (!token.IsCancellationRequested)
         {
            await _cancellation.CancelAsync().ConfigureAwait(false);
         }
      }
      catch (OperationCanceledException)
      {
         // Expected during normal shutdown.
      }
   }
}

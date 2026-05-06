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
   private static readonly TimeSpan DefaultMonitorIntervalInternal = TimeSpan.FromSeconds(1);

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
      using var pipe = new NamedPipeServerStream(_configuration.PipeName, PipeDirection.InOut, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);

      Logger.Information("Listening for transport connection: {PipeName}", _configuration.PipeName);
      await pipe.WaitForConnectionAsync(linked.Token).ConfigureAwait(false);

      Logger.Information("Transport connection established. Initializing Language Server...");

      var server = await InitializeServerAsync(pipe).ConfigureAwait(false);

      // NOTE: We initiate background monitoring of the pipe connection to ensure
      // the server self-terminates if the host process (IDE) closes unexpectedly
      // without sending a formal 'shutdown' request.
      var monitor = Task.Run(() => MonitorPipeConnectionInternalAsync(pipe, linked.Token), CancellationToken.None);

      Logger.Information("Language server initialization complete. Service is running.");

      var completed = await Task.WhenAny(server.WaitForExit, monitor).ConfigureAwait(false);
      if (completed == monitor && !token.IsCancellationRequested)
      {
         Logger.Warning("The transport connection was closed unexpectedly. Stopping the language server.");
      }

      await _cancellation.CancelAsync().ConfigureAwait(false);

      if (pipe.IsConnected)
      {
         await pipe.DisposeAsync().ConfigureAwait(false);
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

   private async Task MonitorPipeConnectionInternalAsync(NamedPipeServerStream stream, CancellationToken cancellationToken)
   {
      try
      {
         while (!cancellationToken.IsCancellationRequested)
         {
            if (!stream.IsConnected)
            {
               await _cancellation.CancelAsync().ConfigureAwait(false);
               return;
            }

            await Task.Delay(DefaultMonitorIntervalInternal, cancellationToken);
         }
      }
      catch (OperationCanceledException)
      {
         // Expected during normal shutdown.
      }
   }
}

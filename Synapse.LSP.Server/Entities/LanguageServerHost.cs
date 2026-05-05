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
      using var pipe = new NamedPipeServerStream(_configuration.PipeName, PipeDirection.InOut, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);

      Logger.Information("Listening for transport connection: {PipeName}", _configuration.PipeName);
      await pipe.WaitForConnectionAsync();

      Logger.Information("Transport connection established. Initializing Language Server...");

      var server = await LanguageServer.From(options =>
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

      // NOTE: We initiate background monitoring of the pipe connection to ensure 
      // the server self-terminates if the host process (IDE) closes unexpectedly 
      // without sending a formal 'shutdown' request.
      _ = Task.Run(() => MonitorPipeConnectionInternalAsync(pipe, _cancellation), _cancellation.Token);

      Logger.Information("Language server initialization complete. Service is running.");
      await server.WaitForExit.ConfigureAwait(false);

      return ExitCodes.Success;
   }

   private async Task MonitorPipeConnectionInternalAsync(NamedPipeServerStream stream, CancellationTokenSource cancellation)
   {
      try
      {
         while (stream.IsConnected && !cancellation.Token.IsCancellationRequested)
         {
            await Task.Delay(DefaultMonitorIntervalInternal, cancellation.Token);
         }
      }
      catch (OperationCanceledException)
      {
         _ = Task.CompletedTask;
      }
   }
}

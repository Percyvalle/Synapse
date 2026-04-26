using System.IO.Pipes;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Server;
using Serilog;
using Synapse.Common.Constants;
using Synapse.LSP.Abstractions.Interfaces;
using Synapse.LSP.Abstractions.Models;
using Synapse.LSP.Handlers;
using ILogger = Serilog.ILogger;

namespace Synapse.LSP.Entities;

public class LanguageServerHost : ILanguageServer
{
   private readonly LanguageServerConfiguration _configuration;

   public LanguageServerHost(LanguageServerConfiguration configuration)
   {
      _configuration = configuration;
   }

   private ILogger Logger { get; } = Log.ForContext<LanguageServerHost>();

   /// <inheritdoc/>
   public async Task<int> RunAsync(CancellationToken token = default)
   {
      var pipe = new NamedPipeServerStream(_configuration.PipeName, PipeDirection.InOut, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);

      Logger.Information("Waiting for client connection on named pipe: {PipeName}...", _configuration.PipeName);
      await pipe.WaitForConnectionAsync();
      Logger.Information("Client connected to pipe. Building Language Server...");

      var server = await LanguageServer.From(options =>
         options
            .WithInput(pipe)
            .WithOutput(pipe)
            .ConfigureLogging(
               builder => builder
                  .AddSerilog()
                  .AddLanguageProtocolLogging()
                  .SetMinimumLevel(LogLevel.Debug))
            .WithHandler<SynapseDocumentHandler>())
         .ConfigureAwait(false);

      Logger.Information("LSP Server initialized and running.");
      await server.WaitForExit.ConfigureAwait(false);

      return ExitCodes.Success;
   }

   /// <inheritdoc/>
   public ValueTask DisposeAsync()
   {
      throw new NotImplementedException();
   }
}

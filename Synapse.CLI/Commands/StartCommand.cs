using System.ComponentModel;
using Serilog;
using Synapse.CLI.Abstractions.Attributes;
using Synapse.CLI.Abstractions.Interfaces;
using Synapse.CLI.Commands.Options;
using Synapse.Common.Constants;
using Synapse.LSP.Abstractions.Interfaces;
using Synapse.LSP.Entities;

namespace Synapse.CLI.Commands;

/// <summary>
/// Provides the execution logic for the "start" command, 
/// responsible for initializing and running the LSP server instance.
/// </summary>
[Command("start")]
[Description("Starts the Synapse Language Server and listens for incoming connections.")]
internal class StartCommand : IExecutableCommand<StartCommandOptions>
{
   private ILogger Logger { get; } = Log.ForContext<StartCommand>();

   /// <inheritdoc/>
   public async Task<int> ExecuteAsync(StartCommandOptions options, CancellationToken token = default)
   {
      Logger.Information("Starting LSP server on pipe: {PipeName}", options.Pipe);

      try
      {
         ILanguageServer server = new LanguageServerHost();
         return await server.RunAsync(token);
      }
      catch (Exception exception)
      {
         Logger.Error(exception, "Failed to start LSP server on pipe {PipeName}", options.Pipe);
         return ExitCodes.Failure;
      }
   }
}

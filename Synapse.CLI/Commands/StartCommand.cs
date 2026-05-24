using System.ComponentModel;
using Serilog;
using Synapse.CLI.Abstractions.Attributes;
using Synapse.CLI.Abstractions.Interfaces;
using Synapse.CLI.Commands.Options;
using Synapse.Common.Constants;
using Synapse.LSP.Abstractions.Interfaces;
using Synapse.LSP.Abstractions.Models;
using Synapse.LSP.Server.Entities;

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
      Logger.Information("Initializing Synapse LSP server instance on pipe: {PipeName}", options.PipeName);

      try
      {
         var configuration = new LanguageServerConfiguration
         {
            PipeName = options.PipeName,
         };

         ILanguageServer server = new LanguageServerHost(configuration);
         return await server.RunAsync(token);
      }
      catch (Exception exception)
      {
         Logger.Fatal(exception, "Critical failure during Synapse LSP startup on pipe {PipeName}", options.PipeName);
         return ExitCodes.Failure;
      }
   }
}

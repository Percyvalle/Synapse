using System.ComponentModel;
using System.Diagnostics;
using System.IO.Pipes;
using Serilog;
using Synapse.CLI.Abstractions.Attributes;
using Synapse.CLI.Abstractions.Interfaces;
using Synapse.CLI.Commands.Options;
using Synapse.Common.Constants;
using Synapse.LSP.Abstractions.Interfaces;
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
         while (!Debugger.IsAttached)
         {
            await Task.Delay(100);
         }

         Debugger.Break();

         using var cancel = CancellationTokenSource.CreateLinkedTokenSource(token);
         using var stream = new NamedPipeServerStream(options.PipeName, PipeDirection.InOut, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);
         await stream.WaitForConnectionAsync(cancel.Token);

         ILanguageServer server = new LanguageServerHost(stream, stream, cancel);
         return await server.RunAsync();
      }
      catch (Exception exception)
      {
         Logger.Fatal(exception, "Critical failure during Synapse LSP startup on pipe {PipeName}", options.PipeName);
         return ExitCodes.Failure;
      }
   }
}

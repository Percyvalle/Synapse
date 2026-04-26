using Serilog;
using Synapse.CLI.Commands;
using Synapse.CLI.Entities;
using Synapse.Common;
using Synapse.Common.Constants;

namespace Synapse.LSP;

/// <summary>
/// Static class that serves as the container for the application's entry point.
/// </summary>
public static class EntryPoint
{
   /// <summary>
   /// The main entry point for the application.
   /// </summary>
   /// <param name="args">An array of command-line arguments.</param>
   /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
   public static async Task<int> Main(string[] args)
   {
      Log.Logger = new LoggerConfiguration()
         .MinimumLevel.Debug()
         .WriteTo.Console()
         .WriteTo.File(ApplicationConstants.DefaultLogFileName, rollingInterval: RollingInterval.Day)
         .CreateLogger();

      try
      {
         var context = new CommandLineBuilder(ApplicationConstants.RootCommandDescription);
         return await context
            .RegistryCommand(new StartCommand())
            .InvokeAsync(args);
      }
      catch (Exception exception)
      {
         Log.Fatal(exception, "The Synapse LSP Server terminated unexpectedly");
         return ExitCodes.Failure;
      }
      finally
      {
         await Log.CloseAndFlushAsync();
      }
   }
}

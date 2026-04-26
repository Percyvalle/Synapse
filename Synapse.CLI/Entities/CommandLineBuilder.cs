using System.CommandLine;
using System.CommandLine.Parsing;
using Synapse.CLI.Abstractions.Interfaces;
using Synapse.CLI.Utilities;

namespace Synapse.CLI.Entities;

/// <summary>
/// Provides a high-level context for managing and executing CLI commands.
/// This class acts as a container for the command tree and handles the dispatching of arguments.
/// </summary>
internal class CommandLineBuilder
{
   private readonly RootCommand _root;

   /// <summary>
   /// Initializes a new instance of the <see cref="CommandLineBuilder"/> class with a specified description.
   /// </summary>
   /// <param name="description">The description of the root command, usually displayed in the help output.</param>
   public CommandLineBuilder(string description)
   {
      _root = new RootCommand(description);
   }

   /// <summary>
   /// Registers a command instance into the command-line application tree.
   /// The command is built using reflection to bind its properties and handlers.
   /// </summary>
   /// <param name="instance">The application command implementation to register.</param>
   /// <returns>The current <see cref="CommandLineBuilder"/> instance for fluent chaining.</returns>
   public CommandLineBuilder RegistryCommand(IApplicationCommand instance)
   {
      _root.Subcommands.Add(instance.Build());
      return this;
   }

   /// <summary>
   /// Parses the provided arguments and executes the matched command asynchronously.
   /// </summary>
   /// <param name="args">The list of command-line arguments (usually passed from the Main method).</param>
   /// <returns>The exit code resulting from the command execution (0 for success, non-zero for errors).</returns>
   public async Task<int> InvokeAsync(IReadOnlyList<string> args)
   {
      return await CommandLineParser.Parse(_root, args).InvokeAsync();
   }
}

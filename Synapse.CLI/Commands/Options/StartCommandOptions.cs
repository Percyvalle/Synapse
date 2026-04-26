using System.ComponentModel;
using Synapse.CLI.Abstractions.Attributes;
using Synapse.CLI.Abstractions.Interfaces.Options;

namespace Synapse.CLI.Commands.Options;

/// <summary>
/// Represents the command-line arguments and configuration options. 
/// for the Language Server Protocol (LSP) "start" command.
/// </summary>
internal class StartCommandOptions : IStartCommandOptions
{
   /// <inheritdoc/>
   [Option("--pipe")]
   [Description("The name of the named pipe used for Inter-Process Communication (IPC).")]
   public string PipeName { get; set; } = "default";
}

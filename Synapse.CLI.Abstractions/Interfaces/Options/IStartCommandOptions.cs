namespace Synapse.CLI.Abstractions.Interfaces.Options;

/// <summary>
/// Defines the configuration contract for the Language Server Protocol (LSP) host.
/// </summary>
public interface IStartCommandOptions
{
   /// <summary>
   /// Gets the name or path of the Named Pipe used for Inter-Process Communication (IPC).
   /// </summary>
   string PipeName { get; }
}

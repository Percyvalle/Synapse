namespace Synapse.LSP.Abstractions.Interfaces;

/// <summary>
/// Defines the core contract for managing the lifecycle of the Synapse Language Server.
/// </summary>
public interface ILanguageServer
{
   /// <summary>
   /// Starts the main message processing loop of the server.
   /// </summary>
   /// <returns>A task resulting in the process exit code (0 for success).</returns>
   Task<int> RunAsync();
}

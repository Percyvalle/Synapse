namespace Synapse.LSP.Abstractions.Interfaces;

/// <summary>
/// Defines the core contract for managing the lifecycle of the Synapse Language Server.
/// </summary>
public interface ILanguageServer : IAsyncDisposable
{
   /// <summary>
   /// Starts the main message processing loop of the server.
   /// </summary>
   /// <param name="token">A cancellation token used to request a graceful shutdown.</param>
   /// <returns>A task resulting in the process exit code (0 for success).</returns>
   Task<int> RunAsync(CancellationToken token = default);
}

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.LanguageServer.Client;

namespace Synapse.LSP.Client.Interfaces;

/// <summary>
/// Defines a service responsible for managing the lifecycle and communication
/// channel of an external Language Server Protocol (LSP) process.
/// </summary>
public interface ILanguageServerLauncher : IAsyncDisposable
{
   /// <summary>
   /// Starts the LSP server process and establishes a connection for JSON-RPC communication.
   /// </summary>
   /// <param name="token">A cancellation token that can be used to abort the startup and connection process.</param>
   /// <returns>
   /// A <see cref="Task{Connection}"/> representing the asynchronous operation.
   /// The task result contains a <see cref="Connection"/> object used by Visual Studio to send and receive LSP messages.
   /// </returns>
   Task<Connection> ActivateAsync(CancellationToken token);
}

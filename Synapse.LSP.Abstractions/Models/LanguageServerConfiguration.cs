namespace Synapse.LSP.Abstractions.Models;

/// <summary>
/// Represents the configuration parameters required to initialize and run the LSP server.
/// </summary>
public class LanguageServerConfiguration
{
   /// <summary>
   /// Gets the name of the Named Pipe that the server will use
   /// for listening to incoming JSON-RPC messages.
   /// </summary>
   public string PipeName { get; init; } = string.Empty;
}

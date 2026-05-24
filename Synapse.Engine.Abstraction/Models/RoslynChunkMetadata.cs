namespace Synapse.Engine.Abstraction.Models;

/// <summary>
/// Represents metadata for a specific chunk of source code parsed by Roslyn.
/// This metadata is stored alongside vector embeddings in the database to provide context
/// for LLM inference and location tracking for Language Server Protocol (LSP) diagnostics.
/// </summary>
public class RoslynChunkMetadata
{
   /// <summary>
   /// Gets the absolute file path or Document URI where this code chunk is located.
   /// Essential for routing LSP diagnostics (e.g., error highlights) back to the correct file in the editor.
   /// </summary>
   public string FilePath { get; init; } = string.Empty;

   /// <summary>
   /// Gets the raw, unformatted source code of the chunk.
   /// Stored here so it can be injected directly into the LLM prompt without needing to re-read the file from disk or cache.
   /// </summary>
   public string Code { get; init; } = string.Empty;

   /// <summary>
   /// Gets the starting line number (zero-based) of the code chunk in the original source file.
   /// Used by the LSP client to determine exactly where to place visual markers.
   /// </summary>
   public int StartLine { get; init; }

   /// <summary>
   /// Gets the ending line number (zero-based) of the code chunk in the original source file.
   /// Used by the LSP client to determine the end range of visual markers.
   /// </summary>
   public int EndLine { get; init; }

   /// <summary>
   /// Gets the syntactic category of this code chunk (e.g., "Class", "Method", "Interface", "Constructor").
   /// Useful for metadata filtering during vector searches (e.g., "search only within methods").
   /// </summary>
   public string ElementType { get; init; } = string.Empty;

   /// <summary>
   /// Gets the specific identifier name of the code element (e.g., "ProcessUserOrder", "IUserService").
   /// Provides additional context for the search results and debugging.
   /// </summary>
   public string ElementName { get; init; } = string.Empty;
}

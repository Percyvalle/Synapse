using Microsoft.CodeAnalysis;

namespace Synapse.Engine.Abstraction.Extensions;

/// <summary>
/// Provides extension methods for extracting source location information from syntax nodes.
/// </summary>
internal static class SyntaxNodeExtensions
{
   /// <summary>
   /// Extracts the zero-based starting line number of the syntax node in its source file.
   /// </summary>
   /// <param name="node">The syntax node to extract the start line from.</param>
   /// <returns>The zero-based line number where the node begins.</returns>
   public static int ExtractStartLine(this SyntaxNode node) =>
      node.GetLocation().GetLineSpan().StartLinePosition.Line;

   /// <summary>
   /// Extracts the zero-based ending line number of the syntax node in its source file.
   /// </summary>
   /// <param name="node">The syntax node to extract the end line from.</param>
   /// <returns>The zero-based line number where the node ends.</returns>
   public static int ExtractEndLine(this SyntaxNode node) =>
      node.GetLocation().GetLineSpan().EndLinePosition.Line;
}

using Microsoft.CodeAnalysis;

/// <summary>
/// Provides extension methods for working with syntax token lists.
/// </summary>
internal static class SyntaxTokenListExtensions
{
   /// <summary>
   /// Converts a syntax token list to a list of token text strings.
   /// </summary>
   /// <param name="tokens">The syntax token list to convert.</param>
   /// <returns>A list of text representations of each token (e.g., "public", "static", "readonly").</returns>
   public static List<string> ToTextList(this SyntaxTokenList tokens) =>
       tokens.Select(t => t.Text).ToList();
}

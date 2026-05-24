using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Synapse.Engine.Extensions;

/// <summary>
/// Provides extension methods for working with base list syntax nodes.
/// </summary>
internal static class BaseListExtensions
{
   /// <summary>
   /// Extracts the names of all base types from the given base list syntax node.
   /// </summary>
   /// <param name="list">The base list syntax node to extract type names from. Can be null.</param>
   /// <returns>A list of base type name strings, or an empty list if the base list is null.</returns>
   public static List<string> ExtractBaseTypeNames(this BaseListSyntax? list) =>
      list?.Types.Select(t => t.Type.ToString()).ToList() ?? new List<string>();
}

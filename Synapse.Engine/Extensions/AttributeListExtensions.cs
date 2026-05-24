using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Synapse.Engine.Extensions;

/// <summary>
/// Provides extension methods for working with attribute list syntax nodes.
/// </summary>
internal static class AttributeListExtensions
{
   /// <summary>
   /// Extracts the names of all attributes from the given attribute list syntax collection.
   /// </summary>
   /// <param name="list">The syntax list of attribute lists to extract names from.</param>
   /// <returns>A list of attribute name strings (e.g., "Obsolete", "Required").</returns>
   public static List<string> ExtractAttributeNames(this SyntaxList<AttributeListSyntax> list) =>
      list.SelectMany(al => al.Attributes)
           .Select(a => a.Name.ToString())
           .ToList();
}

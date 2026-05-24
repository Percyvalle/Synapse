namespace Synapse.Engine.Abstraction.Models.RoslynContext.V1;

/// <summary>
/// Represents a string literal captured from a method body.
/// </summary>
public class RoslynStringLiteralContext
{
   /// <summary>
   /// Gets or sets the string value.
   /// </summary>
   public string Value { get; set; } = string.Empty;

   /// <summary>
   /// Gets or sets the zero-based line number where the literal appears.
   /// </summary>
   public int Line { get; set; }
}

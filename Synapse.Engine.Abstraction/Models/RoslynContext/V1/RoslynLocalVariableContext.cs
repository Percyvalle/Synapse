namespace Synapse.Engine.Abstraction.Models.RoslynContext.V1;

/// <summary>
/// Represents a local variable declaration captured from a method body.
/// </summary>
public class RoslynLocalVariableContext
{
   /// <summary>
   /// Gets or sets the variable name.
   /// </summary>
   public string Name { get; set; } = string.Empty;

   /// <summary>
   /// Gets or sets the declared type (may be "var" for implicitly typed locals).
   /// </summary>
   public string Type { get; set; } = string.Empty;

   /// <summary>
   /// Gets or sets the zero-based line number of the declaration.
   /// </summary>
   public int Line { get; set; }
}

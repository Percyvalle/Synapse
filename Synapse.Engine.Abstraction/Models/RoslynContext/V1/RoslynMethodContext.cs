namespace Synapse.Engine.Abstraction.Models.RoslynContext.V1;

/// <summary>
/// Represents the context of a method extracted via Roslyn analysis.
/// </summary>
public class RoslynMethodContext
{
   /// <summary>
   /// Gets or sets the name of the method.
   /// </summary>
   public string Name { get; set; } = string.Empty;

   /// <summary>
   /// Gets or sets the return type of the method (e.g., "void", "string", "Task").
   /// </summary>
   public string ReturnType { get; set; } = string.Empty;

   /// <summary>
   /// Gets or sets the list of modifiers applied to the method (e.g., "public", "static", "async").
   /// </summary>
   public List<string> Modifiers { get; set; } = new List<string>();

   /// <summary>
   /// Gets or sets the list of attributes decorating the method (e.g., "[HttpGet]", "[Obsolete]").
   /// </summary>
   public List<string> Attributes { get; set; } = new List<string>();

   /// <summary>
   /// Gets or sets the list of parameters defined in the method signature.
   /// </summary>
   public List<RoslynParameterContext> Parameters { get; set; } = new List<RoslynParameterContext>();

   /// <summary>
   /// Gets or sets the line number in the source file where the method declaration begins.
   /// </summary>
   public int StartLine { get; set; }

   /// <summary>
   /// Gets or sets the line number in the source file where the method declaration ends.
   /// </summary>
   public int EndLine { get; set; }
}

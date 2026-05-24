namespace Synapse.Engine.Abstraction.Models.RoslynContext.V1;

/// <summary>
/// Represents the context of a class member extracted via Roslyn analysis.
/// </summary>
public class RoslynMemberContext
{
   /// <summary>
   /// Gets or sets the name of the member (e.g., method name, property name).
   /// </summary>
   public string Name { get; set; } = string.Empty;

   /// <summary>
   /// Gets or sets the type of the member (e.g., return type or property type).
   /// </summary>
   public string Type { get; set; } = string.Empty;

   /// <summary>
   /// Gets or sets the list of modifiers applied to the member (e.g., "public", "static", "readonly").
   /// </summary>
   public List<string> Modifiers { get; set; } = new List<string>();

   /// <summary>
   /// Gets or sets the list of attributes decorating the member (e.g., "[Obsolete]", "[Required]").
   /// </summary>
   public List<string> Attributes { get; set; } = new List<string>();

   /// <summary>
   /// Gets or sets the line number in the source file where the member declaration begins.
   /// </summary>
   public int StartLine { get; set; }

   /// <summary>
   /// Gets or sets the line number in the source file where the member declaration ends.
   /// </summary>
   public int EndLine { get; set; }
}

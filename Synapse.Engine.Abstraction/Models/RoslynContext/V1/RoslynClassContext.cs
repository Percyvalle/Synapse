namespace Synapse.Engine.Abstraction.Models.RoslynContext.V1;

/// <summary>
/// Represents the context of a class extracted via Roslyn analysis.
/// </summary>
public class RoslynClassContext
{
   /// <summary>
   /// Gets or sets the name of the class.
   /// </summary>
   public string Name { get; set; } = string.Empty;

   /// <summary>
   /// Gets or sets the name of the parent class. Used to identify nested class ownership.
   /// </summary>
   public string Parent { get; set; } = string.Empty; // Important for nested classes

   /// <summary>
   /// Gets or sets the list of modifiers applied to the class (e.g., "public", "abstract", "sealed").
   /// </summary>
   public IReadOnlyList<string> Modifiers { get; set; } = new List<string>();

   /// <summary>
   /// Gets or sets the list of base types the class inherits from or implements (e.g., base class or interfaces).
   /// </summary>
   public IReadOnlyList<string> BaseTypes { get; set; } = new List<string>();

   /// <summary>
   /// Gets or sets the list of attributes decorating the class (e.g., "[Serializable]", "[ApiController]").
   /// </summary>
   public IReadOnlyList<string> Attributes { get; set; } = new List<string>();

   /// <summary>
   /// Gets or sets the list of fields declared in the class.
   /// </summary>
   public List<RoslynMemberContext> Fields { get; set; } = new List<RoslynMemberContext>();

   /// <summary>
   /// Gets or sets the list of properties declared in the class.
   /// </summary>
   public List<RoslynMemberContext> Properties { get; set; } = new List<RoslynMemberContext>();

   /// <summary>
   /// Gets or sets the list of methods declared in the class.
   /// </summary>
   public List<RoslynMethodContext> Methods { get; set; } = new List<RoslynMethodContext>();

   /// <summary>
   /// Gets or sets the line number in the source file where the class declaration begins.
   /// </summary>
   public int StartLine { get; set; }

   /// <summary>
   /// Gets or sets the line number in the source file where the class declaration ends.
   /// </summary>
   public int EndLine { get; set; }
}

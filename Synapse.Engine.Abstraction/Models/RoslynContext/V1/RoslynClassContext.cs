namespace Synapse.Engine.Abstraction.Models.RoslynContext.V1;

public class RoslynClassContext
{
   public string Name { get; set; } = string.Empty;

   public string Parent { get; set; } = string.Empty; // Important for nested classes

   public List<string> Modifiers { get; set; } = new List<string>();

   public List<string> BaseTypes { get; set; } = new List<string>();

   public List<string> Attributes { get; set; } = new List<string>();

   public List<RoslynMemberContext> Fields { get; set; } = new List<RoslynMemberContext>();

   public List<RoslynMemberContext> Properties { get; set; } = new List<RoslynMemberContext>();

   public int StartLine { get; set; }

   public int EndLine { get; set; }
}

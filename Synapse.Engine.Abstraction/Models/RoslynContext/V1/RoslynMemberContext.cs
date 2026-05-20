namespace Synapse.Engine.Abstraction.Models.RoslynContext.V1;

public class RoslynMemberContext
{
   public string Name { get; set; } = string.Empty;

   public string Type { get; set; } = string.Empty;

   public List<string> Modifiers { get; set; } = new List<string>();

   public int StartLine { get; set; }

   public int EndLine { get; set; }
}

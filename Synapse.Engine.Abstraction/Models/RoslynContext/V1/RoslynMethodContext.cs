namespace Synapse.Engine.Abstraction.Models.RoslynContext.V1;

public class RoslynMethodContext
{
   public string Name { get; set; } = string.Empty;

   public string ReturnType { get; set; } = string.Empty;

   public List<string> Modifiers { get; set; } = new List<string>();
}

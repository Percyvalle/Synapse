namespace Synapse.Engine.Abstraction.Models.RoslynContext.V1;

/// <summary>
/// Represents the context of a method parameter extracted via Roslyn analysis.
/// </summary>
public class RoslynParameterContext
{
   /// <summary>
   /// Gets or sets the name of the parameter.
   /// </summary>
   public string Name { get; set; } = string.Empty;

   /// <summary>
   /// Gets or sets the type of the parameter (e.g., "int", "string", "CancellationToken").
   /// </summary>
   public string Type { get; set; } = string.Empty;
}

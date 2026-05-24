namespace Synapse.Engine.Abstraction.Models.RoslynContext.V1;

/// <summary>
/// Represents a method invocation captured from a method body.
/// </summary>
public class RoslynInvocationContext
{
   /// <summary>
   /// Gets or sets the name of the invoked method.
   /// </summary>
   public string MethodName { get; set; } = string.Empty;

   /// <summary>
   /// Gets or sets the full expression text of the invocation.
   /// </summary>
   public string Expression { get; set; } = string.Empty;

   /// <summary>
   /// Gets or sets the zero-based line number where the invocation appears.
   /// </summary>
   public int Line { get; set; }

   /// <summary>
   /// Gets or sets the zero-based column number where the invocation begins.
   /// </summary>
   public int Column { get; set; }
}

namespace Synapse.Engine.Abstraction.Models.RoslynContext.V1;

/// <summary>
/// Represents behavioral patterns extracted from a method body.
/// </summary>
public class RoslynMethodBodyContext
{
   /// <summary>
   /// Gets or sets all method invocations found in the body.
   /// </summary>
   public IReadOnlyList<RoslynInvocationContext> Invocations { get; set; } = new List<RoslynInvocationContext>();

   /// <summary>
   /// Gets or sets blocking async calls (.Result, .Wait(), .GetAwaiter().GetResult()).
   /// </summary>
   public IReadOnlyList<RoslynInvocationContext> BlockingCalls { get; set; } = new List<RoslynInvocationContext>();

   /// <summary>
   /// Gets or sets string literals found in the body.
   /// </summary>
   public IReadOnlyList<RoslynStringLiteralContext> StringLiterals { get; set; } = new List<RoslynStringLiteralContext>();

   /// <summary>
   /// Gets or sets local variable declarations found in the body.
   /// </summary>
   public IReadOnlyList<RoslynLocalVariableContext> LocalVariables { get; set; } = new List<RoslynLocalVariableContext>();

   /// <summary>
   /// Gets or sets the number of await expressions in the body.
   /// </summary>
   public int AwaitCount { get; set; }
}

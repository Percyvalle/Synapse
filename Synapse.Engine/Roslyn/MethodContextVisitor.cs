using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Synapse.Engine.Abstraction.Models.RoslynContext.V1;
using Synapse.Engine.Extensions;

namespace Synapse.Engine.Roslyn;

/// <summary>
/// A Roslyn syntax walker that traverses a method body and collects behavioral patterns
/// such as invocations, blocking async calls, string literals, and local variable declarations.
/// </summary>
public class MethodContextVisitor : CSharpSyntaxWalker
{
   private readonly List<RoslynInvocationContext> _invocations = new List<RoslynInvocationContext>();

   private readonly List<RoslynInvocationContext> _blocking = new List<RoslynInvocationContext>();

   private readonly List<RoslynStringLiteralContext> _literals = new List<RoslynStringLiteralContext>();

   private readonly List<RoslynLocalVariableContext> _variables = new List<RoslynLocalVariableContext>();

   private int _await;

   /// <summary>
   /// Builds a <see cref="RoslynMethodBodyContext"/> from all patterns collected during traversal.
   /// </summary>
   /// <returns>The collected behavioral context of the visited method body.</returns>
   public RoslynMethodBodyContext Build() => new RoslynMethodBodyContext()
   {
      Invocations = _invocations,
      BlockingCalls = _blocking,
      AwaitCount = _await,
      StringLiterals = _literals,
      LocalVariables = _variables,
   };

   /// <inheritdoc/>
   public override void VisitInvocationExpression(InvocationExpressionSyntax node)
   {
      if (node.Expression is not MemberAccessExpressionSyntax member)
      {
         base.VisitInvocationExpression(node);
         return;
      }

      var invocation = new RoslynInvocationContext
      {
         MethodName = member.Name.Identifier.Text,
         Expression = node.ToString(),
         Line = node.ExtractStartLine(),
         Column = node.ExtractStartColumn(),
      };

      _invocations.Add(invocation);

      if (IsBlockingInvocation(member.Name.Identifier.Text))
      {
         _blocking.Add(invocation);
      }

      base.VisitInvocationExpression(node);
   }

   /// <inheritdoc/>
   public override void VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
   {
      if (node.Name.Identifier.Text != "Result" ||
          node.Parent is InvocationExpressionSyntax)
      {
         base.VisitMemberAccessExpression(node);
         return;
      }

      _blocking.Add(new RoslynInvocationContext
      {
         MethodName = "Result",
         Expression = node.ToString(),
         Line = node.ExtractStartLine(),
         Column = node.ExtractStartColumn(),
      });

      base.VisitMemberAccessExpression(node);
   }

   /// <inheritdoc/>
   public override void VisitAwaitExpression(AwaitExpressionSyntax node)
   {
      _await++;
      base.VisitAwaitExpression(node);
   }

   /// <inheritdoc/>
   public override void VisitLiteralExpression(LiteralExpressionSyntax node)
   {
      if (!node.IsKind(SyntaxKind.StringLiteralExpression))
      {
         base.VisitLiteralExpression(node);
         return;
      }

      _literals.Add(new RoslynStringLiteralContext
      {
         Value = node.Token.ValueText,
         Line = node.ExtractStartLine(),
      });

      base.VisitLiteralExpression(node);
   }

   /// <inheritdoc/>
   public override void VisitLocalDeclarationStatement(LocalDeclarationStatementSyntax node)
   {
      foreach (var variable in node.Declaration.Variables)
      {
         _variables.Add(new RoslynLocalVariableContext
         {
            Name = variable.Identifier.Text,
            Type = node.Declaration.Type.ToString(),
            Line = node.ExtractStartLine(),
         });
      }

      base.VisitLocalDeclarationStatement(node);
   }

   private static bool IsBlockingInvocation(string method) =>
        method is "Wait" or "GetResult";
}

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Synapse.Engine.Abstraction.Models.RoslynContext.V1;
using Synapse.Engine.Roslyn;

namespace Synapse.Engine.Tests;

/// <summary>
/// Contains unit tests for the <see cref="MethodContextVisitor"/> class.
/// </summary>
public class MethodContextVisitorTests
{
   [Fact]
   public void Visit_WithEmptyBody_ReturnsEmptyCollectionsAndZeroAwait()
   {
      // Arrange
      var source = """
         void Method() { }
         """;

      // Act
      var body = Visit(source);

      // Assert
      Assert.Empty(body.Invocations);
      Assert.Empty(body.BlockingCalls);
      Assert.Empty(body.StringLiterals);
      Assert.Empty(body.LocalVariables);
      Assert.Equal(0, body.AwaitCount);
   }

   [Fact]
   public void Visit_WithRegularInvocation_AddsToInvocationsOnly()
   {
      // Arrange
      var source = """
         void Method() { console.WriteLine("test"); }
         """;

      // Act
      var body = Visit(source);

      // Assert
      Assert.Contains(body.Invocations, i => i.MethodName == "WriteLine");
      Assert.Empty(body.BlockingCalls);
   }

   [Fact]
   public void Visit_WithWaitCall_AddsToInvocationsAndBlockingCalls()
   {
      // Arrange
      var source = """
         void Method() { someTask.Wait(); }
         """;

      // Act
      var body = Visit(source);

      // Assert
      Assert.Contains(body.Invocations, i => i.MethodName == "Wait");
      Assert.Contains(body.BlockingCalls, i => i.MethodName == "Wait");
   }

   [Fact]
   public void Visit_WithResultAccess_AddsToBlockingCallsOnly()
   {
      // Arrange
      var source = """
         void Method() { var x = someTask.Result; }
         """;

      // Act
      var body = Visit(source);

      // Assert
      Assert.Contains(body.BlockingCalls, i => i.MethodName == "Result");
      Assert.DoesNotContain(body.Invocations, i => i.MethodName == "Result");
   }

   [Fact]
   public void Visit_WithGetAwaiterGetResult_AddsGetResultToBlockingCalls()
   {
      // Arrange
      var source = """
         void Method() { var x = someTask.GetAwaiter().GetResult(); }
         """;

      // Act
      var body = Visit(source);

      // Assert
      Assert.Contains(body.BlockingCalls, i => i.MethodName == "GetResult");
      Assert.Contains(body.Invocations, i => i.MethodName == "GetAwaiter");
      Assert.Contains(body.Invocations, i => i.MethodName == "GetResult");
   }

   [Fact]
   public void Visit_WithSingleAwait_SetsAwaitCountToOne()
   {
      // Arrange
      var source = """
         async Task Method() { await Task.Delay(100); }
         """;

      // Act
      var body = Visit(source);

      // Assert
      Assert.Equal(1, body.AwaitCount);
   }

   [Fact]
   public void Visit_WithMultipleAwaits_CountsAll()
   {
      // Arrange
      var source = """
         async Task Method()
         {
            await A();
            await B();
            await C();
         }
         """;

      // Act
      var body = Visit(source);

      // Assert
      Assert.Equal(3, body.AwaitCount);
   }

   [Fact]
   public void Visit_WithStringLiteral_CollectsValue()
   {
      // Arrange
      var source = """
         void Method() { var s = "hello"; }
         """;

      // Act
      var body = Visit(source);

      // Assert
      Assert.Contains(body.StringLiterals, l => l.Value == "hello");
   }

   [Fact]
   public void Visit_WithMultipleStringLiterals_CollectsAll()
   {
      // Arrange
      var source = """
         void Method()
         {
            var a = "foo";
            var b = "bar";
         }
         """;

      // Act
      var body = Visit(source);

      // Assert
      Assert.Equal(2, body.StringLiterals.Count);
      Assert.Contains(body.StringLiterals, l => l.Value == "foo");
      Assert.Contains(body.StringLiterals, l => l.Value == "bar");
   }

   [Fact]
   public void Visit_WithExplicitTypeVariable_CollectsNameAndType()
   {
      // Arrange
      var source = """
         void Method() { string name = "test"; }
         """;

      // Act
      var body = Visit(source);

      // Assert
      var variable = Assert.Single(body.LocalVariables, v => v.Name == "name");
      Assert.Equal("string", variable.Type);
   }

   [Fact]
   public void Visit_WithVarVariable_CollectsVarAsType()
   {
      // Arrange
      var source = """
         void Method() { var x = GetValue(); }
         """;

      // Act
      var body = Visit(source);

      // Assert
      var variable = Assert.Single(body.LocalVariables);
      Assert.Equal("x", variable.Name);
      Assert.Equal("var", variable.Type);
   }

   [Fact]
   public void Visit_WithMultipleVariablesInOneDeclaration_SplitsIntoSeparateEntries()
   {
      // Arrange
      var source = """
         void Method() { int a = 1, b = 2; }
         """;

      // Act
      var body = Visit(source);

      // Assert
      Assert.Equal(2, body.LocalVariables.Count);
      Assert.Contains(body.LocalVariables, v => v.Name == "a" && v.Type == "int");
      Assert.Contains(body.LocalVariables, v => v.Name == "b" && v.Type == "int");
   }

   [Fact]
   public void Visit_WithInvocation_CapturesCorrectLineNumber()
   {
      // Arrange
      var source = """
         void Method()
         {
            someTask.Wait();
         }
         """;

      // Act
      var body = Visit(source);

      // Assert
      var call = Assert.Single(body.Invocations);
      Assert.Equal(2, call.Line);
   }

   [Fact]
   public void Visit_WithExpressionBody_AnalyzesExpression()
   {
      // Arrange
      var source = """
         int Method() => someService.GetValue();
         """;

      // Act
      var body = Visit(source);

      // Assert
      Assert.Contains(body.Invocations, i => i.MethodName == "GetValue");
   }

   private static RoslynMethodBodyContext Visit(string source)
   {
      var tree = CSharpSyntaxTree.ParseText("class C {" + source + "}");
      var method = tree.GetRoot().DescendantNodes().OfType<MethodDeclarationSyntax>().First();
      var visitor = new MethodContextVisitor();

      if (method.Body != null)
      {
         visitor.Visit(method.Body);
      }
      else if (method.ExpressionBody != null)
      {
         visitor.Visit(method.ExpressionBody);
      }

      return visitor.Build();
   }
}

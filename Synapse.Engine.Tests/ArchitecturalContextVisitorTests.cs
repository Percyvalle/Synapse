using Microsoft.CodeAnalysis.CSharp;
using Synapse.Engine.Roslyn;

namespace Synapse.Engine.Tests;

/// <summary>
/// Contains unit tests for the <see cref="ArchitecturalContextVisitor"/> class.
/// </summary>
public class ArchitecturalContextVisitorTests
{
   [Fact]
   public void Visit_WithUsingDirectives_CollectsAllNamespaces()
   {
      // Arrange
      var source = """
         using System;
         using System.Collections.Generic;
         using Synapse.Engine;
         """;

      // Act
      var visitor = Visit(source);

      // Assert
      Assert.Equal(3, visitor.Usings.Count);
      Assert.Contains("System", visitor.Usings);
      Assert.Contains("System.Collections.Generic", visitor.Usings);
      Assert.Contains("Synapse.Engine", visitor.Usings);
   }

   [Fact]
   public void Visit_WithoutUsingDirectives_ReturnsEmptyUsingsList()
   {
      // Arrange
      var source = "class Foo { }";

      // Act
      var visitor = Visit(source);

      // Assert
      Assert.Empty(visitor.Usings);
   }

   [Fact]
   public void Visit_WithSingleClass_CollectsNameAndModifiers()
   {
      // Arrange
      var source = """
         public sealed class MyClass
         {
         }
         """;

      // Act
      var visitor = Visit(source);

      // Assert
      var cls = Assert.Single(visitor.Classes);
      Assert.Equal("MyClass", cls.Name);
      Assert.Equal(string.Empty, cls.Parent);
      Assert.Equal(2, cls.Modifiers.Count);
      Assert.Contains("public", cls.Modifiers);
      Assert.Contains("sealed", cls.Modifiers);
   }

   [Fact]
   public void Visit_WithClassBaseTypes_CollectsBaseAndInterfaces()
   {
      // Arrange
      var source = """
         public class MyService : BaseService, IService, IDisposable
         {
         }
         """;

      // Act
      var visitor = Visit(source);

      // Assert
      var cls = Assert.Single(visitor.Classes);
      Assert.Equal(3, cls.BaseTypes.Count);
      Assert.Contains("BaseService", cls.BaseTypes);
      Assert.Contains("IService", cls.BaseTypes);
      Assert.Contains("IDisposable", cls.BaseTypes);
   }

   [Fact]
   public void Visit_WithClassAttributes_CollectsAttributeNames()
   {
      // Arrange
      var source = """
         [Controller]
         [Route("api/[controller]")]
         public class UsersController
         {
         }
         """;

      // Act
      var visitor = Visit(source);

      // Assert
      var cls = Assert.Single(visitor.Classes);
      Assert.Equal(2, cls.Attributes.Count);
      Assert.Contains("Controller", cls.Attributes);
      Assert.Contains("Route", cls.Attributes);
   }

   [Fact]
   public void Visit_WithNestedClass_SetsParentNameOnInnerClass()
   {
      // Arrange
      var source = """
         public class Outer
         {
            public class Inner
            {
            }
         }
         """;

      // Act
      var visitor = Visit(source);

      // Assert
      Assert.Equal(2, visitor.Classes.Count);

      var outer = visitor.Classes.Single(c => c.Name == "Outer");
      Assert.Equal(string.Empty, outer.Parent);

      var inner = visitor.Classes.Single(c => c.Name == "Inner");
      Assert.Equal("Outer", inner.Parent);
   }

   [Fact]
   public void Visit_WithDeeplyNestedClasses_SetsCorrectParentChain()
   {
      // Arrange
      var source = """
         public class A
         {
            public class B
            {
               public class C
               {
               }
            }
         }
         """;

      // Act
      var visitor = Visit(source);

      // Assert
      Assert.Equal(string.Empty, visitor.Classes.Single(c => c.Name == "A").Parent);
      Assert.Equal("A", visitor.Classes.Single(c => c.Name == "B").Parent);
      Assert.Equal("B", visitor.Classes.Single(c => c.Name == "C").Parent);
   }

   [Fact]
   public void Visit_WithClassDeclaration_ReturnsOneBasedLineNumbers()
   {
      // Arrange
      var source = """
         using System;

         public class MyClass
         {
         }
         """;

      // Act
      var visitor = Visit(source);

      // Assert
      var cls = Assert.Single(visitor.Classes);
      Assert.Equal(2, cls.StartLine);
      Assert.Equal(4, cls.EndLine);
   }

   [Fact]
   public void Visit_WithInterface_CollectsAsClassContext()
   {
      // Arrange
      var source = """
         public interface IRepository : IDisposable
         {
         }
         """;

      // Act
      var visitor = Visit(source);

      // Assert
      var iface = Assert.Single(visitor.Classes);
      Assert.Equal("IRepository", iface.Name);
      Assert.Contains("public", iface.Modifiers);
      Assert.Contains("IDisposable", iface.BaseTypes);
   }

   [Fact]
   public void Visit_WithMethod_AddsMethodToContainingClass()
   {
      // Arrange
      var source = """
         public class Calculator
         {
            public int Add(int a, int b) => a + b;
         }
         """;

      // Act
      var visitor = Visit(source);

      // Assert
      var cls = Assert.Single(visitor.Classes);
      var method = Assert.Single(cls.Methods);
      Assert.Equal("Add", method.Name);
      Assert.Equal("int", method.ReturnType);
      Assert.Contains("public", method.Modifiers);
      Assert.Equal(2, method.Parameters.Count);
      Assert.Equal("a", method.Parameters[0].Name);
      Assert.Equal("int", method.Parameters[0].Type);
      Assert.Equal("b", method.Parameters[1].Name);
      Assert.Equal("int", method.Parameters[1].Type);
   }

   [Fact]
   public void Visit_WithAsyncMethod_CollectsModifiersAndReturnType()
   {
      // Arrange
      var source = """
         public class Service
         {
            public async Task<string> GetAsync(CancellationToken token)
            {
               return await Task.FromResult("ok");
            }
         }
         """;

      // Act
      var visitor = Visit(source);

      // Assert
      var method = Assert.Single(visitor.Classes[0].Methods);
      Assert.Contains("public", method.Modifiers);
      Assert.Contains("async", method.Modifiers);
      Assert.Equal("Task<string>", method.ReturnType);
      Assert.Contains(method.Parameters, p => p.Type == "CancellationToken");
   }

   [Fact]
   public void Visit_WithMethodAttributes_CollectsAttributeNames()
   {
      // Arrange
      var source = """
         public class Controller
         {
            [HttpGet]
            [Authorize]
            public void Get() { }
         }
         """;

      // Act
      var visitor = Visit(source);

      // Assert
      var method = Assert.Single(visitor.Classes[0].Methods);
      Assert.Equal(2, method.Attributes.Count);
      Assert.Contains("HttpGet", method.Attributes);
      Assert.Contains("Authorize", method.Attributes);
   }

   [Fact]
   public void Visit_WithMethodInNestedClass_AddsMethodToInnerClassOnly()
   {
      // Arrange
      var source = """
         public class Outer
         {
            public void OuterMethod() { }

            public class Inner
            {
               public void InnerMethod() { }
            }
         }
         """;

      // Act
      var visitor = Visit(source);

      // Assert
      var outer = visitor.Classes.Single(c => c.Name == "Outer");
      var inner = visitor.Classes.Single(c => c.Name == "Inner");

      Assert.Single(outer.Methods);
      Assert.Equal("OuterMethod", outer.Methods[0].Name);

      Assert.Single(inner.Methods);
      Assert.Equal("InnerMethod", inner.Methods[0].Name);
   }

   [Fact]
   public void Visit_WithSingleField_CollectsNameTypeAndModifiers()
   {
      // Arrange
      var source = """
         public class Foo
         {
            private readonly string _name;
         }
         """;

      // Act
      var visitor = Visit(source);

      // Assert
      var field = Assert.Single(visitor.Classes[0].Fields);
      Assert.Equal("_name", field.Name);
      Assert.Equal("string", field.Type);
      Assert.Contains("private", field.Modifiers);
      Assert.Contains("readonly", field.Modifiers);
   }

   [Fact]
   public void Visit_WithMultipleVariableField_SplitsIntoSeparateMembers()
   {
      // Arrange
      var source = """
         public class Foo
         {
            public int a, b, c;
         }
         """;

      // Act
      var visitor = Visit(source);

      // Assert
      var fields = visitor.Classes[0].Fields;
      Assert.Equal(3, fields.Count);
      Assert.Contains(fields, f => f.Name == "a");
      Assert.Contains(fields, f => f.Name == "b");
      Assert.Contains(fields, f => f.Name == "c");
      Assert.All(fields, f => Assert.Equal("int", f.Type));
      Assert.All(fields, f => Assert.Contains("public", f.Modifiers));
   }

   [Fact]
   public void Visit_WithFieldAttribute_CollectsAttributeName()
   {
      // Arrange
      var source = """
         public class Foo
         {
            [JsonIgnore]
            private int _hidden;
         }
         """;

      // Act
      var visitor = Visit(source);

      // Assert
      var field = Assert.Single(visitor.Classes[0].Fields);
      Assert.Contains("JsonIgnore", field.Attributes);
   }

   [Fact]
   public void Visit_WithProperty_CollectsNameTypeAndModifiers()
   {
      // Arrange
      var source = """
         public class Foo
         {
            public string Name { get; set; } = "";
         }
         """;

      // Act
      var visitor = Visit(source);

      // Assert
      var prop = Assert.Single(visitor.Classes[0].Properties);
      Assert.Equal("Name", prop.Name);
      Assert.Equal("string", prop.Type);
      Assert.Contains("public", prop.Modifiers);
   }

   [Fact]
   public void Visit_WithPropertyAttribute_CollectsAttributeName()
   {
      // Arrange
      var source = """
         public class Foo
         {
            [Required]
            public string Email { get; set; } = "";
         }
         """;

      // Act
      var visitor = Visit(source);

      // Assert
      var prop = Assert.Single(visitor.Classes[0].Properties);
      Assert.Contains("Required", prop.Attributes);
   }

   [Fact]
   public void Visit_WithEmptySource_ReturnsEmptyCollections()
   {
      // Arrange
      var source = string.Empty;

      // Act
      var visitor = Visit(source);

      // Assert
      Assert.Empty(visitor.Classes);
      Assert.Empty(visitor.Usings);
   }

   [Fact]
   public void Visit_WithMultipleTopLevelTypes_CollectsAll()
   {
      // Arrange
      var source = """
         public class First { }
         public class Second { }
         public interface IThird { }
         """;

      // Act
      var visitor = Visit(source);

      // Assert
      Assert.Equal(3, visitor.Classes.Count);
      Assert.Contains(visitor.Classes, c => c.Name == "First");
      Assert.Contains(visitor.Classes, c => c.Name == "Second");
      Assert.Contains(visitor.Classes, c => c.Name == "IThird");
   }

   private static ArchitecturalContextVisitor Visit(string source)
   {
      var tree = CSharpSyntaxTree.ParseText(source);
      var visitor = new ArchitecturalContextVisitor();
      visitor.Visit(tree.GetRoot());
      return visitor;
   }
}

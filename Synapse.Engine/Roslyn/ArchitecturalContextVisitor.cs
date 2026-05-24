using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Synapse.Engine.Abstraction.Models.RoslynContext.V1;
using Synapse.Engine.Extensions;

namespace Synapse.Engine.Roslyn;

/// <summary>
/// A Roslyn syntax walker that traverses C# syntax trees and collects
/// architectural context information about classes, interfaces, and methods.
/// </summary>
internal class ArchitecturalContextVisitor : CSharpSyntaxWalker
{
   private readonly Stack<RoslynClassContext> _stack = new Stack<RoslynClassContext>();

   /// <summary>
   /// Gets the list of using directive namespaces collected from the syntax tree.
   /// </summary>
   public List<string> Usings { get; } = new List<string>();

   /// <summary>
   /// Gets the list of class and interface contexts collected from the syntax tree.
   /// </summary>
   public List<RoslynClassContext> Classes { get; } = new List<RoslynClassContext>();

   /// <inheritdoc/>
   public override void VisitUsingDirective(UsingDirectiveSyntax node)
   {
      var name = node.Name?.ToString();
      if (!string.IsNullOrEmpty(name))
      {
         Usings.Add(name);
      }

      base.VisitUsingDirective(node);
   }

   /// <inheritdoc/>
   public override void VisitClassDeclaration(ClassDeclarationSyntax node)
   {
      var parent = _stack.Count > 0 ? _stack.Peek().Name : string.Empty;
      var context = new RoslynClassContext
      {
         Name = node.Identifier.Text,
         Parent = parent,
         Modifiers = node.Modifiers.ToTextList(),
         BaseTypes = node.BaseList.ExtractBaseTypeNames(),
         Attributes = node.AttributeLists.ExtractAttributeNames(),
         StartLine = node.ExtractStartLine(),
         EndLine = node.ExtractEndLine(),
      };

      Classes.Add(context);
      _stack.Push(context);

      base.VisitClassDeclaration(node);

      _stack.Pop();
   }

   /// <inheritdoc/>
   public override void VisitInterfaceDeclaration(InterfaceDeclarationSyntax node)
   {
      var parent = _stack.Count > 0 ? _stack.Peek().Name : string.Empty;
      var context = new RoslynClassContext
      {
         Name = node.Identifier.Text,
         Parent = parent,
         Modifiers = node.Modifiers.ToTextList(),
         BaseTypes = node.BaseList.ExtractBaseTypeNames(),
         Attributes = node.AttributeLists.ExtractAttributeNames(),
         StartLine = node.ExtractStartLine(),
         EndLine = node.ExtractEndLine(),
      };

      Classes.Add(context);
      _stack.Push(context);

      base.VisitInterfaceDeclaration(node);

      _stack.Pop();
   }

   /// <inheritdoc/>
   public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
   {
      if (_stack.Count <= 0)
      {
         base.VisitMethodDeclaration(node);
         return;
      }

      var context = new RoslynMethodContext
      {
         Name = node.Identifier.Text,
         ReturnType = node.ReturnType.ToString(),
         Modifiers = node.Modifiers.ToTextList(),
         Attributes = node.AttributeLists.ExtractAttributeNames(),
         Parameters = node.ParameterList.Parameters
            .Select(param => new RoslynParameterContext
            {
               Name = param.Identifier.Text,
               Type = param.Type?.ToString() ?? string.Empty,
            })
            .ToList(),
         StartLine = node.ExtractStartLine(),
         EndLine = node.ExtractEndLine(),
      };

      _stack.Peek().Methods.Add(context);

      base.VisitMethodDeclaration(node);
   }

   /// <inheritdoc/>
   public override void VisitFieldDeclaration(FieldDeclarationSyntax node)
   {
      if (_stack.Count <= 0)
      {
         base.VisitFieldDeclaration(node);
         return;
      }

      foreach (var variable in node.Declaration.Variables)
      {
         _stack.Peek().Fields.Add(new RoslynMemberContext
         {
            Name = variable.Identifier.Text,
            Type = node.Declaration.Type.ToString(),
            Modifiers = node.Modifiers.ToTextList(),
            Attributes = node.AttributeLists.ExtractAttributeNames(),
            StartLine = node.ExtractStartLine(),
            EndLine = node.ExtractEndLine(),
         });
      }

      base.VisitFieldDeclaration(node);
   }

   /// <inheritdoc/>
   public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node)
   {
      if (_stack.Count <= 0)
      {
         base.VisitPropertyDeclaration(node);
         return;
      }

      _stack.Peek().Properties.Add(new RoslynMemberContext
      {
         Name = node.Identifier.Text,
         Type = node.Type.ToString(),
         Modifiers = node.Modifiers.ToTextList(),
         Attributes = node.AttributeLists.ExtractAttributeNames(),
         StartLine = node.ExtractStartLine(),
         EndLine = node.ExtractEndLine(),
      });

      base.VisitPropertyDeclaration(node);
   }
}

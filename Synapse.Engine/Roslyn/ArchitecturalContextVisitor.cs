using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Synapse.Engine.Roslyn;

internal class ArchitecturalContextVisitor : CSharpSyntaxWalker
{
   public override void VisitClassDeclaration(ClassDeclarationSyntax node)
   {
      base.VisitClassDeclaration(node);
   }

   public override void VisitInterfaceDeclaration(InterfaceDeclarationSyntax node)
   {
      base.VisitInterfaceDeclaration(node);
   }

   public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
   {
      base.VisitMethodDeclaration(node);
   }

   public override void VisitUsingDirective(UsingDirectiveSyntax node)
   {
      base.VisitUsingDirective(node);
   }
}

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace burtonrodman
{
    public class ServiceClassSyntaxReceiver : ISyntaxReceiver
    {
        public List<ClassDeclarationSyntax> ServiceClassDeclarations { get; } = new();

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (syntaxNode is ClassDeclarationSyntax cds && cds.ShouldGenerateConstructor())
            {
                ServiceClassDeclarations.Add(cds);
            }
        }
    }
}

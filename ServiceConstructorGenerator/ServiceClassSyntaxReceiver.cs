using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace burtonrodman
{
    public class ServiceClassSyntaxReceiver : ISyntaxReceiver
    {
        public ClassDeclarationSyntax ServiceClassDeclaration { get; private set; }

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (syntaxNode is ClassDeclarationSyntax cds && cds.ShouldGenerateConstructor())
            {
                ServiceClassDeclaration = cds;
            }
        }
    }
}

using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace burtonrodman.FieldInjectionGenerator
{
    [Generator]
    public class ConstructorSourceGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context) { }

        public void Execute(GeneratorExecutionContext context)
        {
            // if (!Debugger.IsAttached) Debugger.Launch();

            var attributeHasBeenGenerated = false;

            foreach (var tree in context.Compilation.SyntaxTrees)
            {
                foreach (var classDeclaration in tree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>())
                {
                    if (classDeclaration.ShouldGenerateConstructor())
                    {
                        try
                        {
                            if (!attributeHasBeenGenerated)
                            {
                                context.GenerateAttributeCode();
                                attributeHasBeenGenerated = true;
                            }

                            context.AddSource($"{classDeclaration.Identifier.Text}.g.cs",
                                classDeclaration.GenerateFieldInjectionConstructor(tree.GetContainingNamespace()));
                        }
                        catch (Exception ex)
                        {
                            // context.AddSource($"{classDeclaration.Identifier.Text}.g.cs", ex.ToString());

                            var descriptor = new DiagnosticDescriptor(
                                "SCG001", "An error occurred generating a constructor for your service.",
                                "{0}", "Source Generation", DiagnosticSeverity.Error, true);

                            context.ReportDiagnostic(Diagnostic.Create(descriptor, classDeclaration.GetLocation(), ex.ToString()));
                        }
                    }
                }
            }
        }
    }
}

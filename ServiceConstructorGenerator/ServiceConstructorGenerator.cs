using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace burtonrodman
{
    [Generator]
    public class ServiceConstructorGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new ServiceClassSyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            //if (!Debugger.IsAttached) Debugger.Launch();

            context.GenerateAttributeCode();

            // The SyntaxReceiver first get a chance to filter the nodes that we care about
            // now we need to loop over the collected nodes and generate the source files.

            if (context.SyntaxReceiver is ServiceClassSyntaxReceiver receiver)
            {
                foreach (var classDeclaration in receiver.ServiceClassDeclarations)
                {
                    try
                    {
                        context.AddSource($"{classDeclaration.GetContainingNamespace()}.{classDeclaration.Identifier.Text}.g.cs",
                            classDeclaration.GenerateServiceConstructor(context));
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

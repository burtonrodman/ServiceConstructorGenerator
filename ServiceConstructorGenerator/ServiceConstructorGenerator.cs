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
        public void Initialize(GeneratorInitializationContext context) {
            context.RegisterForSyntaxNotifications(() => new ServiceClassSyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
             //if (!Debugger.IsAttached) Debugger.Launch();

            var attributeHasBeenGenerated = false;

            var receiver = (ServiceClassSyntaxReceiver)context.SyntaxReceiver;
            var classDeclaration = receiver.ServiceClassDeclaration;
            try
            {
                if (!attributeHasBeenGenerated)
                {
                    context.GenerateAttributeCode();
                    attributeHasBeenGenerated = true;
                }

                context.AddSource($"{classDeclaration.Identifier.Text}.g.cs",
                    classDeclaration.GenerateFieldInjectionConstructor(context));
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

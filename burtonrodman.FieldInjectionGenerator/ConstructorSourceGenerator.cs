using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace burtonrodman.FieldInjectionGenerator
{
    [Generator]
    public class ConstructorSourceGenerator : ISourceGenerator
    {
        public const string AttributeName = "GenerateFieldInjectionConstructor";

        public void Initialize(GeneratorInitializationContext context) { }

        public void Execute(GeneratorExecutionContext context)
        {
            //if (!Debugger.IsAttached) Debugger.Launch();
            var attributeHasBeenGenerated = false;

            foreach (var tree in context.Compilation.SyntaxTrees)
            {
                foreach (var classDeclaration in tree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>())
                {
                    var attr = classDeclaration.DescendantNodes().OfType<AttributeSyntax>().FirstOrDefault();
                    if (attr?.Name is IdentifierNameSyntax name && name.Identifier.Text == AttributeName)
                    {
                        try
                        {
                            var namespaceSyntax = tree.GetRoot().DescendantNodes()
                                .OfType<NamespaceDeclarationSyntax>()
                                .FirstOrDefault().Name as IdentifierNameSyntax;

                            var readonlyFields = classDeclaration.DescendantNodes().OfType<FieldDeclarationSyntax>()
                                .Where(f => f.Modifiers.Any(m => m.IsKind(SyntaxKind.ReadOnlyKeyword)));
                            var constructorParams = new List<string>();
                            var constructorAssignments = new List<string>();
                            foreach (var field in readonlyFields)
                            {
                                var type = field.Declaration.Type switch
                                {
                                    IdentifierNameSyntax id => id.Identifier.Text,
                                    PredefinedTypeSyntax pd => pd.Keyword.Text,
                                    _ => "UNKNOWN"
                                };
                                var fieldName = (field.Declaration.Variables.First() as VariableDeclaratorSyntax).Identifier.Text;

                                constructorParams.Add($"{type} {fieldName}");
                                constructorAssignments.Add($"this.{fieldName} = {fieldName};");
                            }

                            string source =
$@"// Auto-generated code
using System;
namespace {namespaceSyntax.Identifier.Text}
{{
    public partial class {classDeclaration.Identifier.Text}
    {{
        public {classDeclaration.Identifier.Text}(
            {string.Join(", ", constructorParams)}
        ) {{
{string.Join("\r\n", constructorAssignments)}
        }}
    }}
}}
";

                            context.AddSource($"{classDeclaration.Identifier.Text}.g.cs", source);
                            if (!attributeHasBeenGenerated)
                            {
                                GenerateAttributeCode(context);
                                attributeHasBeenGenerated = true;
                            }
                        }
                        catch (Exception ex)
                        {
                            context.AddSource($"{classDeclaration.Identifier.Text}.g.cs", ex.ToString());
                        }
                    }
                }
            }
        }

        private static void GenerateAttributeCode(GeneratorExecutionContext context)
        {
            string source =
$@"
using System;

namespace burtonrodman.FieldInjectionGenerator
{{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class GenerateFieldInjectionConstructorAttribute : Attribute
    {{
    }}
}}
";
            context.AddSource("GenerateFieldInjectionConstructorAttribute.g.cs", source);

        }
    }
}

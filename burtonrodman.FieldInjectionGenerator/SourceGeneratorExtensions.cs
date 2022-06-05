using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace burtonrodman.FieldInjectionGenerator
{
    public static class SourceGeneratorExtensions
    {
        public const string ConstructorAttributeName = "GenerateFieldInjectionConstructor";
        public const string InjectAsOptionsAttributeName = "InjectAsOptions";

        public static bool ShouldGenerateConstructor(this ClassDeclarationSyntax classDeclaration)
        {
            var attr = classDeclaration.DescendantNodes().OfType<AttributeSyntax>().FirstOrDefault();
            return attr?.Name is IdentifierNameSyntax name && name.Identifier.Text == ConstructorAttributeName;
        }

        public static string GetContainingNamespace(this SyntaxTree tree)
        {
            var namespaceSyntax = tree.GetRoot().DescendantNodes()
                .OfType<NamespaceDeclarationSyntax>()
                .FirstOrDefault().Name as IdentifierNameSyntax;
            return namespaceSyntax.Identifier.Text;
        }

        public static string GetTypeName(this FieldDeclarationSyntax field)
            => field.Declaration.Type switch
            {
                IdentifierNameSyntax id => id.Identifier.Text,
                PredefinedTypeSyntax pd => pd.Keyword.Text,
                _ => "UNKNOWN"
            };

        public static string GetVariableName(this FieldDeclarationSyntax field)
            => (field.Declaration.Variables.First() as VariableDeclaratorSyntax).Identifier.Text;

        public static (List<string> constructorParams, List<string> constructorAssignments)
            GetConstructorParameters(this ClassDeclarationSyntax classDeclaration)
        {
            var readonlyFields = classDeclaration.DescendantNodes().OfType<FieldDeclarationSyntax>()
                .Where(f => f.Modifiers.Any(m => m.IsKind(SyntaxKind.ReadOnlyKeyword)));
            var constructorParams = new List<string>();
            var constructorAssignments = new List<string>();
            foreach (var field in readonlyFields)
            {
                var type = field.GetTypeName();
                var fieldName = field.GetVariableName();

                if (field.ShouldInjectAsOptions())
                {
                    constructorParams.Add($"Microsoft.Extensions.Options.IOptions<{type}> {fieldName}");
                    constructorAssignments.Add($"this.{fieldName} = {fieldName}.Value;");
                }
                else
                {
                    constructorParams.Add($"{type} {fieldName}");
                    constructorAssignments.Add($"this.{fieldName} = {fieldName};");
                }
            }

            return (constructorParams, constructorAssignments);
        }

        public static bool ShouldInjectAsOptions(this FieldDeclarationSyntax field)
        {
            return field.AttributeLists.SelectMany(l => l.Attributes)
                .Any(a => a.Name is IdentifierNameSyntax name && name.Identifier.Text == InjectAsOptionsAttributeName);
        }

        public static string GenerateFieldInjectionConstructor(
            this ClassDeclarationSyntax classDeclaration,
            string containingNamespace
        )
        {
            var (constructorParams, constructorAssignments) = classDeclaration.GetConstructorParameters();
            return $@"// Auto-generated code
using System;
namespace {containingNamespace}
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
        }

        
        public static void GenerateAttributeCode(this GeneratorExecutionContext context)
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

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class InjectAsOptionsAttribute : Attribute
    {{
    }}
}}
";
            context.AddSource("FieldInjectionGeneratorAttributes.g.cs", source);
        }

    }
}

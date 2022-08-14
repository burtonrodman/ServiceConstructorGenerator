using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace burtonrodman
{
    public static class SourceGeneratorExtensions
    {
        public const string ConstructorAttributeName = "GenerateServiceConstructor";
        public const string InjectAsOptionsAttributeName = "InjectAsOptions";

        public static bool ShouldGenerateConstructor(this ClassDeclarationSyntax classDeclaration)
        {
            var attr = classDeclaration.DescendantNodes().OfType<AttributeSyntax>().FirstOrDefault();
            return attr?.Name is IdentifierNameSyntax name && name.Identifier.Text == ConstructorAttributeName;
        }

        public static IList<string> GetAllUsingStatements(this ClassDeclarationSyntax classDeclaration)
        {
            if (classDeclaration.GetCompilationUnitSyntax() is CompilationUnitSyntax root)
            {
                return root.Usings.Select(u => u.ToString()).ToList();
            }
            return new List<string>();
        }

        public static CompilationUnitSyntax? GetCompilationUnitSyntax(this SyntaxNode node)
        {
            if (node.Parent is CompilationUnitSyntax parent) return parent;

            if (node.Parent is SyntaxNode parentSyntax)
                return node.Parent.GetCompilationUnitSyntax();

            return null;
        }

        public static string GetContainingNamespace(this ClassDeclarationSyntax classDeclaration)
        {
            if (classDeclaration.Parent is BaseNamespaceDeclarationSyntax namespaceDeclaration)
            {
                return (namespaceDeclaration.Name switch
                {
                    IdentifierNameSyntax id => id.Identifier.Text,
                    QualifiedNameSyntax fq => fq.GetText().ToString(),
                    _ => throw new InvalidOperationException($"The namespace's identifier was not the expected type.  It was type {namespaceDeclaration.Name.GetType().Name}.")
                }).Trim();
            }
            throw new InvalidOperationException("could not determine containing namespace");
        }

        public static string GetTypeName(this FieldDeclarationSyntax field)
            => field.Declaration.Type switch
            {
                IdentifierNameSyntax id => id.Identifier.Text,
                PredefinedTypeSyntax pd => pd.Keyword.Text,
                _ => field.Declaration.Type.ToString()
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

        public static string GenerateServiceConstructor(
            this ClassDeclarationSyntax classDeclaration,
            GeneratorExecutionContext context
        )
        {
            var usings = classDeclaration.GetAllUsingStatements();
            var (constructorParams, constructorAssignments) = classDeclaration.GetConstructorParameters();
            return $@"
// Auto-generated code
using System;
{string.Join("\r\n", usings)}
namespace {classDeclaration.GetContainingNamespace()}
{{
    public partial class {classDeclaration.Identifier.Text}
    {{
        public {classDeclaration.Identifier.Text}(
            {string.Join(", \n\t\t\t\t\t\t", constructorParams)}
        ) {{
            {string.Join("\r\n\t\t\t\t\t\t", constructorAssignments)}
        }}
    }}
}}
";
        }

        public const string AttributesSourceFileName = "ServiceConstructorGeneratorAttributes.g.cs";
        public const string AttributesSourceCode =
$@"
using System;

namespace burtonrodman.ServiceConstructorGenerator
{{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class GenerateServiceConstructorAttribute : Attribute
    {{
    }}

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class InjectAsOptionsAttribute : Attribute
    {{
    }}
}}
";

        public static void GenerateAttributeCode(this GeneratorExecutionContext context)
        {
            context.AddSource(AttributesSourceFileName, AttributesSourceCode);
        }

    }
}

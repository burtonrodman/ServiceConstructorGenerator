using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace burtonrodman;

public static class SourceGeneratorExtensions
{
    public const string ConstructorAttributeName = "GenerateServiceConstructor";
    private static readonly string[] InjectAsAttributeNames = { "InjectAs", "InjectAsOptions" };

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
            return parentSyntax.GetCompilationUnitSyntax();

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

    public static string GetTypeName(this FieldDeclarationSyntax field) => field.Declaration.Type.GetTypeName();
    public static string GetTypeName(this PropertyDeclarationSyntax prop) => prop.Type.GetTypeName();
    public static string GetTypeName(this TypeSyntax type)
        => type switch
        {
            IdentifierNameSyntax id => id.Identifier.Text,
            PredefinedTypeSyntax pd => pd.Keyword.Text,
            _ => type.ToString()
        };

    public static string GetVariableName(this FieldDeclarationSyntax field)
        => (field.Declaration.Variables.First() as VariableDeclaratorSyntax).Identifier.Text;

    public static string GetPropertyName(this PropertyDeclarationSyntax prop)
        => (prop.Identifier.Text);

    public static (List<string> constructorParams, List<string> constructorAssignments)
        GetConstructorParameters(this ClassDeclarationSyntax classDeclaration)
    {

        var readonlyFields = classDeclaration.DescendantNodes().OfType<FieldDeclarationSyntax>()
            .Where(f => f.Modifiers.Any(m => m.IsKind(SyntaxKind.ReadOnlyKeyword) ||
                        f.Modifiers.Any(m => m.IsKind(SyntaxKind.RequiredKeyword))))
            .Select(f => new ParameterInfo() { 
                TypeName = f.GetTypeName(), 
                MemberName = f.GetVariableName(), 
                InjectAs = f.GetInjectAs(f.GetTypeName(), f.GetVariableName()),
                DeclarationStartingLine = f.Span.Start
            });

        var requiredProperties = classDeclaration.DescendantNodes().OfType<PropertyDeclarationSyntax>()
            .Where(p => p.Modifiers.Any(m => m.IsKind(SyntaxKind.RequiredKeyword)))
            .Select(p => new ParameterInfo() { 
                TypeName = p.GetTypeName(), 
                MemberName = p.GetPropertyName(), 
                InjectAs = p.GetInjectAs(p.GetTypeName(), p.GetPropertyName()),
                DeclarationStartingLine = p.Span.Start
            });

        var constructorParams = new List<string>();
        var constructorAssignments = new List<string>();
        foreach (var info in readonlyFields.Union(requiredProperties).OrderBy(m => m.DeclarationStartingLine))
        {
            constructorParams.Add($"{info.InjectAs.TypeName} {info.MemberName}");
            constructorAssignments.Add($"this.{info.MemberName} = {info.InjectAs.InitExpression} ?? throw new ArgumentNullException(nameof({info.MemberName}));");
        }

        return (constructorParams, constructorAssignments);
    }

    public static (string TypeName, string InitExpression) GetInjectAs(this FieldDeclarationSyntax field, string typeName, string memberName) => field.AttributeLists.GetInjectAs(typeName, memberName);
    public static (string TypeName, string InitExpression) GetInjectAs(this PropertyDeclarationSyntax prop, string typeName, string memberName) => prop.AttributeLists.GetInjectAs(typeName, memberName);

    public static (string TypeName, string InitExpression) GetInjectAs(this SyntaxList<AttributeListSyntax> lists, string typeName, string memberName)
    {
        var attr = lists.SelectMany(l => l.Attributes)
            .FirstOrDefault(a => a.Name is IdentifierNameSyntax name && InjectAsAttributeNames.Contains(name.Identifier.Text));
        if (attr is not null)
        {
            //       [InjectAs<IFooWrapper>(getter: nameof(IFooWrapper.Object))]

            var attrName = ((IdentifierNameSyntax)attr.Name).Identifier.Text;
            return attrName switch {
                "InjectAsOptions" => ($"Microsoft.Extensions.Options.IOptions<{typeName}>", $"{memberName}.Value"),
                "InjectAs" => ("foo", "bar"),
                _ => throw new NotSupportedException()
            };
        }
        else
        {
            return (typeName, memberName);
        }
    }

    public static string GenerateServiceConstructor(
        this ClassDeclarationSyntax classDeclaration,
        GeneratorExecutionContext context
    )
    {
        var usings = classDeclaration.GetAllUsingStatements();
        var (constructorParams, constructorAssignments) = classDeclaration.GetConstructorParameters();
        return $$$"""
               using System;
               {{{string.Join("\r\n", usings)}}}
               namespace {{{classDeclaration.GetContainingNamespace()}}}
               {
                   public partial class {{{classDeclaration.Identifier.Text}}}
                   {
                       [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
                       public {{{classDeclaration.Identifier.Text}}}(
                           {{{string.Join(",\r\n            ", constructorParams)}}}
                       ) {
                           {{{string.Join("\r\n            ", constructorAssignments)}}}
                       }
                   }
               }
               """;
    }

    public const string AttributesSourceFileName = "ServiceConstructorGeneratorAttributes.g.cs";
    public const string AttributesSourceCode =
        """
        using System;

        namespace burtonrodman.ServiceConstructorGenerator
        {
            [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
            public class GenerateServiceConstructorAttribute : Attribute { }

            [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
            public class InjectAsOptionsAttribute : Attribute { }
        }
        """;

    public static void GenerateAttributeCode(this GeneratorExecutionContext context)
    {
        context.AddSource(AttributesSourceFileName, AttributesSourceCode);
    }

}

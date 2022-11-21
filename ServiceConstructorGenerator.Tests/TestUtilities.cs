using burtonrodman;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Immutable;
using System.Text;
using verifycs = SourceGeneratorVerifier<burtonrodman.ServiceConstructorGenerator>;

namespace ServiceConstructorGeneratorTests;

internal static class TestUtilities
{
    public static (Type sourceGeneratorType, string filename, SourceText content) CreateGeneratedSource(string filename, string source)
        => (typeof(ServiceConstructorGenerator), filename, SourceText.From(source, Encoding.UTF8, SourceHashAlgorithm.Sha1));

    public static async Task VerifySourceGenerator(string code, params (Type, string, SourceText)[] generated)
    {
        var verifier = new verifycs.Test()
        {
            TestState =
            {
                ReferenceAssemblies = ReferenceAssemblies.Default
                    .AddPackages(ImmutableArray.Create(
                        new PackageIdentity("Microsoft.Extensions.Options", "7.0.0")
                    )),
                Sources = {
                    code,
                    """
                    using System.ComponentModel;
                    namespace System.Runtime.CompilerServices;
                    [EditorBrowsable(EditorBrowsableState.Never)]
                    internal static class IsExternalInit { }
                    """,
                    """
                    namespace System.Runtime.CompilerServices;
                    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
                    internal sealed class RequiredMemberAttribute : Attribute { }
                    """,
                    """
                    namespace System.Runtime.CompilerServices;
                    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
                    public sealed class CompilerFeatureRequiredAttribute : Attribute
                    {
                        public CompilerFeatureRequiredAttribute(string featureName)
                        {
                            FeatureName = featureName;
                        }
                        public string FeatureName { get; }
                        public bool IsOptional { get; init; }
                        public const string RefStructs = nameof(RefStructs);
                        public const string RequiredMembers = nameof(RequiredMembers);
                    }
                    """
                }
            },
        };
        verifier.TestState.GeneratedSources.Add(
            (typeof(ServiceConstructorGenerator), SourceGeneratorExtensions.AttributesSourceFileName,
                SourceText.From(SourceGeneratorExtensions.AttributesSourceCode, Encoding.UTF8, SourceHashAlgorithm.Sha1))
        );
        foreach (var source in generated)
        {
            verifier.TestState.GeneratedSources.Add(source);
        }

        await verifier.RunAsync();
    }

}

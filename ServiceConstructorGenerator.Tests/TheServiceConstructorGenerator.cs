using burtonrodman;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Immutable;
using System.Text;
using verifycs = SourceGeneratorVerifier<burtonrodman.ServiceConstructorGenerator>;

namespace ServiceConstructorGeneratorTests
{
    public class TheServiceConstructorGenerator
    {
        private (Type sourceGeneratorType, string filename, SourceText content) CreateGeneratedSource(string filename, string source)
            => (typeof(ServiceConstructorGenerator), filename, SourceText.From(source, Encoding.UTF8, SourceHashAlgorithm.Sha1));

        private async Task VerifySourceGenerator(string code, params (Type, string, SourceText)[] generated)
        {
            var verifier = new verifycs.Test()
            {
                TestState =
                {
                    ReferenceAssemblies = ReferenceAssemblies.Default
                        .AddPackages(ImmutableArray.Create(
                            new PackageIdentity("Microsoft.Extensions.Options", "6.0.0")
                        )),
                    Sources = { code }
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

        [Fact]
        public async void DetectsSimpleInputNamespaceFileScoped()
        {
            await VerifySourceGenerator(@"
using burtonrodman.ServiceConstructorGenerator;
namespace ConsoleApp28;

[GenerateServiceConstructor]
public partial class Foo
{
    public readonly int _bar;
}
",

CreateGeneratedSource("ConsoleApp28.Foo.g.cs", @"
// Auto-generated code
using System;
using burtonrodman.ServiceConstructorGenerator;
namespace ConsoleApp28
{
    public partial class Foo
    {
        public Foo(
            int _bar
        ) {
            this._bar = _bar;
        }
    }
}
")
            );
        }


        [Fact]
        public async void DetectsQualifiedInputNamespaceBlock()
        {
            await VerifySourceGenerator(@"
using burtonrodman.ServiceConstructorGenerator;
namespace ConsoleApp28.Baz
{
    [GenerateServiceConstructor]
    public partial class Foo
    {
        public readonly int _bar;
    }
}
",

CreateGeneratedSource("ConsoleApp28.Baz.Foo.g.cs", @"
// Auto-generated code
using System;
using burtonrodman.ServiceConstructorGenerator;
namespace ConsoleApp28.Baz
{
    public partial class Foo
    {
        public Foo(
            int _bar
        ) {
            this._bar = _bar;
        }
    }
}
")
            );
        }


        [Fact]
        public async void WrapsInjectAsOptionsField()
        {
            await VerifySourceGenerator(@"
using burtonrodman.ServiceConstructorGenerator;
namespace ConsoleApp28.Baz
{
    [GenerateServiceConstructor]
    public partial class Foo
    {
        [InjectAsOptions]
        public readonly string _bar;
    }
}
",

CreateGeneratedSource("ConsoleApp28.Baz.Foo.g.cs", @"
// Auto-generated code
using System;
using burtonrodman.ServiceConstructorGenerator;
namespace ConsoleApp28.Baz
{
    public partial class Foo
    {
        public Foo(
            Microsoft.Extensions.Options.IOptions<string> _bar
        ) {
            this._bar = _bar.Value;
        }
    }
}
")
            );
        }

        [Fact]
        public async void QualifiesFieldTypes()
        {
            await VerifySourceGenerator(@"
using burtonrodman.ServiceConstructorGenerator;
using ConsoleApp28.Services;

namespace ConsoleApp28.Services
{
    public interface ITestService
    {
    }
}
namespace ConsoleApp28.Baz
{
    [GenerateServiceConstructor]
    public partial class Foo
    {
        public readonly ITestService _bar;
    }
}
",

CreateGeneratedSource("ConsoleApp28.Baz.Foo.g.cs", @"
// Auto-generated code
using System;
using burtonrodman.ServiceConstructorGenerator;
using ConsoleApp28.Services;
namespace ConsoleApp28.Baz
{
    public partial class Foo
    {
        public Foo(
            ITestService _bar
        ) {
            this._bar = _bar;
        }
    }
}
")
            );
        }
    }

}
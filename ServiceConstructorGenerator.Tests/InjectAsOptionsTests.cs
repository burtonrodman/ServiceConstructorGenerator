namespace ServiceConstructorGeneratorTests;

public class InjectAsOptionsTests
{

    [Fact]
    public async void WrapsInjectAsOptionsField()
    {
        await VerifySourceGenerator(
            """
            namespace ConsoleApp28.Baz
            {
                [GenerateServiceConstructor]
                public partial class Foo
                {
                    [InjectAsOptions]
                    public readonly ITestService _bar;
                }
            }
            """,

            CreateGeneratedSource("ConsoleApp28.Baz.Foo.g.cs",
            """
            using System;
            
            namespace ConsoleApp28.Baz
            {
                public partial class Foo
                {
                    [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
                    public Foo(
                        Microsoft.Extensions.Options.IOptions<ITestService> _bar
                    ) {
                        this._bar = _bar.Value;
                    }
                }
            }
            """)
        );
    }

}
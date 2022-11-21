namespace ServiceConstructorGeneratorTests;

public class InjectAsOptionsTests
{

    [Fact]
    public async void WrapsInjectAsOptionsField()
    {
        await VerifySourceGenerator(
            """
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
            """,

            CreateGeneratedSource("ConsoleApp28.Baz.Foo.g.cs",
            """
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
            """)
        );
    }

}
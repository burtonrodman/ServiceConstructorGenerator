namespace ServiceConstructorGeneratorTests;

public class RequiredPropertyTests
{

    [Fact]
    public async Task GeneratesParametersForRequiredProperties()
    {
        await VerifySourceGenerator(
            """
            using burtonrodman.ServiceConstructorGenerator;
            namespace ConsoleApp28;

            [GenerateServiceConstructor]
            public partial class Foo
            {
                public required int Bar { private get; init; }
            }
            """,

            CreateGeneratedSource("ConsoleApp28.Foo.g.cs",
            """
            // Auto-generated code
            using System;
            using burtonrodman.ServiceConstructorGenerator;
            namespace ConsoleApp28
            {
                public partial class Foo
                {
                    public Foo(
                        int Bar
                    ) {
                        this.Bar = Bar;
                    }
                }
            }
            """)
        );
    }

    [Fact]
    public async Task GeneratesParametersInSourceOrder()
    {
        await VerifySourceGenerator(
            """
            using burtonrodman.ServiceConstructorGenerator;
            namespace ConsoleApp28;

            [GenerateServiceConstructor]
            public partial class Foo
            {
                public required int Zzz { private get; init; }
                public readonly int Yyy;
                public required int Xxx { private get; init; }
                public readonly int Www;
            }
            """,

            CreateGeneratedSource("ConsoleApp28.Foo.g.cs",
            """
            // Auto-generated code
            using System;
            using burtonrodman.ServiceConstructorGenerator;
            namespace ConsoleApp28
            {
                public partial class Foo
                {
                    public Foo(
                        int Zzz,
                        int Yyy,
                        int Xxx,
                        int Www
                    ) {
                        this.Zzz = Zzz;
                        this.Yyy = Yyy;
                        this.Xxx = Xxx;
                        this.Www = Www;
                    }
                }
            }
            """)
        );
    }

}
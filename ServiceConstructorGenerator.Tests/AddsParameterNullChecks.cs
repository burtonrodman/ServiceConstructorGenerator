namespace ServiceConstructorGeneratorTests;

public partial class TheServiceConstructorGenerator
{

    [Fact]
    public async void AddsParameterNullChecks()
    {
        await VerifySourceGenerator(
            """
            namespace ConsoleApp28;

            [GenerateServiceConstructor]
            public partial class Foo
            {
                public readonly ITestService _bar;
            }
            """,

            CreateGeneratedSource("ConsoleApp28.Foo.g.cs",
            """
            using System;

            namespace ConsoleApp28
            {
                public partial class Foo
                {
                    partial void OnAfterInitialized();

                    public Foo(
                        ITestService bar
                    ) {
                        this._bar = bar ?? throw new ArgumentNullException(nameof(bar));

                        OnAfterInitialized();
                    }
                }
            }
            """)
        );
    }
}
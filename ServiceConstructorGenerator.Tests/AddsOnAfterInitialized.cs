namespace ServiceConstructorGeneratorTests;

public partial class TheServiceConstructorGenerator
{

    [Fact]
    public async void AddsOnAfterInitialized()
    {
        await VerifySourceGenerator(
            """
            namespace ConsoleApp28;

            [GenerateServiceConstructor]
            public partial class Foo
            {
                public readonly ITestService _bar;

                partial void OnAfterInitialized()
                {
                    Test();
                }

                private void Test()
                {
                }
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
                        ITestService _bar
                    ) {
                        this._bar = _bar ?? throw new ArgumentNullException(nameof(_bar));

                        OnAfterInitialized();
                    }
                }
            }
            """)
        );
    }
}
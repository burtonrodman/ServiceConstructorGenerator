namespace ServiceConstructorGeneratorTests;

public partial class TheServiceConstructorGenerator
{

    [Fact]
    public async void HandlesClassWithGenericParameter()
    {
        await VerifySourceGenerator(
            """
            namespace ConsoleApp28;

            [GenerateServiceConstructor]
            public partial class Foo<T, U> where T : class
            {
                public readonly ITestService _bar;
            }
            """,

            CreateGeneratedSource("ConsoleApp28.Foo.g.cs",
            """
            using System;

            namespace ConsoleApp28
            {
                public partial class Foo<T, U>
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
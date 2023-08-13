namespace ServiceConstructorGeneratorTests;

public partial class TheServiceConstructorGenerator
{

    [Fact]
    public async void SupportsAllFieldNamePatterns()
    {
        await VerifySourceGenerator(
            """
            namespace ConsoleApp28;

            [GenerateServiceConstructor]
            public partial class Foo<T, U> where T : class
            {
                public readonly ITestService _bar;
                public readonly ITestService baz;
                public readonly ITestService Bam;
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
                        ITestService bar,
                        ITestService baz,
                        ITestService bam
                    ) {
                        this._bar = bar ?? throw new ArgumentNullException(nameof(bar));
                        this.baz = baz ?? throw new ArgumentNullException(nameof(baz));
                        this.Bam = bam ?? throw new ArgumentNullException(nameof(bam));

                        OnAfterInitialized();
                    }
                }
            }
            """)
        );
    }
}
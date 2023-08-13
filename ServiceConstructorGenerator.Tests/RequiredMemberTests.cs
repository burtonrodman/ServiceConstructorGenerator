namespace ServiceConstructorGeneratorTests;

public class RequiredMemberTests
{

    [Fact]
    public async Task GeneratesParametersForRequiredFields()
    {
        await VerifySourceGenerator(
            """
            namespace ConsoleApp28;
            [GenerateServiceConstructor]
            public partial class Foo
            {
                public required ITestService _bar;
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

                    [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
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

    [Fact]
    public async Task GeneratesParametersForRequiredProperties()
    {
        await VerifySourceGenerator(
            """
            namespace ConsoleApp28;

            [GenerateServiceConstructor]
            public partial class Foo
            {
                public required ITestService Bar { private get; init; }
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

                    [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
                    public Foo(
                        ITestService bar
                    ) {
                        this.Bar = bar ?? throw new ArgumentNullException(nameof(bar));

                        OnAfterInitialized();
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
            namespace ConsoleApp28;
            [GenerateServiceConstructor]
            public partial class Foo
            {
                public required ITestService Zzz { private get; init; }
                public readonly ITestService Yyy;
                public required ITestService Xxx { private get; init; }
                public required ITestService Www;
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

                    [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
                    public Foo(
                        ITestService zzz,
                        ITestService yyy,
                        ITestService xxx,
                        ITestService www
                    ) {
                        this.Zzz = zzz ?? throw new ArgumentNullException(nameof(zzz));
                        this.Yyy = yyy ?? throw new ArgumentNullException(nameof(yyy));
                        this.Xxx = xxx ?? throw new ArgumentNullException(nameof(xxx));
                        this.Www = www ?? throw new ArgumentNullException(nameof(www));

                        OnAfterInitialized();
                    }
                }
            }
            """)
        );
    }

}
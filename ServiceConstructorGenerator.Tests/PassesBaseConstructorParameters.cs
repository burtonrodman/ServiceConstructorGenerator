namespace ServiceConstructorGeneratorTests;

public partial class TheServiceConstructorGenerator
{

    [Fact]
    public async void PassesBaseConstructorParameters()
    {
        await VerifySourceGenerator(
            """
            namespace ConsoleApp28;

            public abstract class Command
            {
                protected Command(string name, string description) { }
            }

            [GenerateServiceConstructor("register", "Register stuff")]
            public partial class Foo : Command
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

                    [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
                    public Foo(
                        ITestService _bar
                    ) : base("register", "Register stuff") {
                        this._bar = _bar ?? throw new ArgumentNullException(nameof(_bar));

                        OnAfterInitialized();
                    }
                }
            }
            """)
        );
    }
}
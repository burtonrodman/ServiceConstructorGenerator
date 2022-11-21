namespace ServiceConstructorGeneratorTests;

public class TheServiceConstructorGenerator
{

    [Fact]
    public async void DetectsSimpleInputNamespaceFileScoped()
    {
        await VerifySourceGenerator(
            """
            using burtonrodman.ServiceConstructorGenerator;
            namespace ConsoleApp28;

            [GenerateServiceConstructor]
            public partial class Foo
            {
                public readonly int _bar;
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
                        int _bar
                    ) {
                        this._bar = _bar;
                    }
                }
            }
            """)
        );
    }

    [Fact]
    public async void DetectsQualifiedInputNamespaceBlock()
    {
        await VerifySourceGenerator(
            """
            using burtonrodman.ServiceConstructorGenerator;
            namespace ConsoleApp28.Baz
            {
                [GenerateServiceConstructor]
                public partial class Foo
                {
                    public readonly int _bar;
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
                        int _bar
                    ) {
                        this._bar = _bar;
                    }
                }
            }
            """)
        );
    }

    [Fact]
    public async void QualifiesFieldTypes()
    {
        await VerifySourceGenerator(
            """
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
            """,

            CreateGeneratedSource("ConsoleApp28.Baz.Foo.g.cs",
            """
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
            """)
        );
    }

}
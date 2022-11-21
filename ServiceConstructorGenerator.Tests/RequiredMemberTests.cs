﻿namespace ServiceConstructorGeneratorTests;

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
                    [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
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
                    [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
                    public Foo(
                        ITestService Bar
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
                    [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
                    public Foo(
                        ITestService Zzz,
                        ITestService Yyy,
                        ITestService Xxx,
                        ITestService Www
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
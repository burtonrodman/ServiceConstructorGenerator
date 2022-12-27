namespace ServiceConstructorGeneratorTests;

public class InjectAsOptionsTests
{

    [Fact]
    public async void WrapsInjectAsOptionsFieldOrProperty()
    {
        await VerifySourceGenerator(
            """
            namespace ConsoleApp28.Baz
            {
                [GenerateServiceConstructor]
                public partial class Foo
                {
                    [InjectAsOptions]
                    public readonly ITestService _bar;

                    [InjectAsOptions]
                    public required ITestService OtherTestService { private get; init; }
                }
            }
            """,

            CreateGeneratedSource("ConsoleApp28.Baz.Foo.g.cs",
            """
            using System;
            
            namespace ConsoleApp28.Baz
            {
                public partial class Foo
                {
                    [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
                    public Foo(
                        Microsoft.Extensions.Options.IOptions<ITestService> _bar,
                        Microsoft.Extensions.Options.IOptions<ITestService> OtherTestService
                    ) {
                        this._bar = _bar.Value ?? throw new ArgumentNullException(nameof(_bar));
                        this.OtherTestService = OtherTestService.Value ?? throw new ArgumentNullException(nameof(OtherTestService));
                    }
                }
            }
            """)
        );
    }

    // [Fact]
    // public async void WrapsInjectAsFieldOrProperty()
    // {
    //     await VerifySourceGenerator(
    //         """
    //         namespace ConsoleApp28.Baz;

    //         public interface IFooWrapper
    //         {
    //             public ITestService Object { get; }
    //         }
    //         [GenerateServiceConstructor]
    //         public partial class Foo
    //         {
    //             [InjectAs<IFooWrapper>(getter: nameof(IFooWrapper.Object))]
    //             public required ITestService OtherTestService { private get; init; }
    //         }
    //         """,

    //         CreateGeneratedSource("ConsoleApp28.Baz.Foo.g.cs",
    //         """
    //         using System;
            
    //         namespace ConsoleApp28.Baz
    //         {
    //             public partial class Foo
    //             {
    //                 [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
    //                 public Foo(
    //                     IFooWrapper OtherTestService
    //                 ) {
    //                     this.OtherTestService = OtherTestService.Object;
    //                 }
    //             }
    //         }
    //         """)
    //     );
    // }

}
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
                    partial void OnAfterInitialized();

                    [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
                    public Foo(
                        Microsoft.Extensions.Options.IOptions<ITestService> bar,
                        Microsoft.Extensions.Options.IOptions<ITestService> otherTestService
                    ) {
                        this._bar = bar.Value ?? throw new ArgumentNullException(nameof(bar));
                        this.OtherTestService = otherTestService.Value ?? throw new ArgumentNullException(nameof(otherTestService));

                        OnAfterInitialized();
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
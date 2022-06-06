
namespace ConsoleApp1.TestNamespace.Foo
{

    [GenerateFieldInjectionConstructor]
    public partial class Test
    {
        private readonly IHelloFrom _foo;
        private readonly IFoo2 _foo2;
        private readonly int _blah;
        private readonly IFoo3 _foo3;
        private readonly IFoo3 _foo4;
    }

}
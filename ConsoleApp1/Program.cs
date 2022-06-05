using burtonrodman.FieldInjectionGenerator;

namespace ConsoleApp1
{
    public partial class Program
    {
        static void Main(string[] args)
        {
            HelloFrom("Generated Code");

            var foo2 = new Foo2();
            var foo3 = new Foo3();
            var hello = new HelloFrom(foo2, foo3);
            var foo = new Test(hello, foo2, 5, foo3);
        }

        static partial void HelloFrom(string name);
    }

    [GenerateFieldInjectionConstructor]
    public partial class Test
    {
        private readonly IHelloFrom _foo;
        private readonly IFoo2 _foo2;
        private readonly int _blah;
        private readonly IFoo3 _foo3;
    }

    public interface IHelloFrom
    {
    }

    [GenerateFieldInjectionConstructor]
    public partial class HelloFrom : IHelloFrom
    {
        private readonly IFoo2 _foo2;
        private readonly IFoo3 _foo3;
    }

    public interface IFoo2 { }
    public class Foo2 : IFoo2 { }

    public interface IFoo3 { }
    public class Foo3 : IFoo3 { }
    
}
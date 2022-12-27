using Microsoft.Extensions.Options;

namespace ConsoleApp1
{
    public partial class Program
    {
        static void Main(string[] args)
        {
            HelloFrom("Generated Code");

            var foo2 = new Foo2();
            var foo3 = new Foo3();
            var options = new Options<EmailSenderOptions>(new EmailSenderOptions());
            var hello = new HelloFromClass(foo2, foo3, options);
            var blah = new Blah();
            var foo = new ConsoleApp1.TestNamespace.Foo.Test(hello, foo2, blah, foo3, foo3);
        }

        static partial void HelloFrom(string name);
    }
    public interface IHelloFrom
    {
    } 

    [GenerateServiceConstructor]
    public partial class HelloFromClass : IHelloFrom
    {
        private readonly IFoo2 _foo4;
        private readonly IFoo3 _foo5;
        [InjectAsOptions]
        private readonly EmailSenderOptions _emailSenderOptions;
    }

    public interface IBlah { }
    public class Blah : IBlah { }

    public interface IFoo2 { }
    public class Foo2 : IFoo2 { }

    public interface IFoo3 { }
    public class Foo3 : IFoo3 { }

    public class EmailSenderOptions
    {
        public string? SmtpServer { get; set; }
    }


    public class Options<TOptions> : IOptions<TOptions>
        where TOptions : class
    {
        public TOptions Value { get; }

        public Options(TOptions value)
        {
            Value = value;
        }
    }
}

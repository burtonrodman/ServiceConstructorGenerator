# ServiceConstructorGenerator
A C# Source Generator that generates a constructor to initialize all readonly fields and/or required properties.

This reduces the amount of boiler-plate code needed when using constructor injection in ASP.Net Core projects for example.  However, this can be used with any C# project and does NOT require ASP.Net Core, or even a Dependency Injection system.

Constructor Parameters are generated in source order.

# Getting Started

1. Add the ```burtonrodman.ServiceConstructorGenerator``` NuGet package to your project.
2. Add a using for `burtonrodman.ServiceConstructorGenerator` to the top of your C# file or as a `global using`.
3. Add a `[GenerateServiceConstructor]` attribute to your class.
4. Add the `partial` keyword on your class.
5. Ensure all fields that should be injected are using the `readonly` keyword.
6. Ensure all properties that should be injected are using the `required` keyword (suggestion, scope the property as `public` with a `private get;` and `init;`.).
7. Optionally, add an `[InjectAsOptions]` attribute on any field/property that should be wrapped with IOptions.
8. Optionally, implement `partial void OnAfterInitialized()` in your class.

In this example, the following constructor will be generated:

```
/// Usings.cs
global using burtonrodman.ServiceConstructorGenerator;

```

```
namespace MyApp;

[GenerateServiceConstructor]
public partial class Test
{
    private readonly IHttpContextAccessor _accessor;
    public required IWidgetRepository WidgetRepository { private get; init; };
    [InjectAsOptions]
    private readonly EmailSenderOptions _emailSenderOptions;

    partial void OnAfterInitialized()
    {
        // add your logic here
    }
}
```

Generated Code:
```
namespace MyApp
{
    public partial class Test
    {
        partial void OnAfterInitialized();
        
        public Test(
            IHttpContextAccessor _accessor,
            IWidgetRepository WidgetRepository,
            Microsoft.Extensions.Options.IOptions<EmailSenderOptions> _emailSenderOptions
        ) {
            this._accessor = _accessor ?? throw new ArgumentNullException(nameof(_accessor));
            this.WidgetRepository = WidgetRepository ?? throw new ArgumentNullException(nameof(WidgetRepository));
            this._emailSenderOptions = _emailSenderOptions.Value ?? throw new ArgumentNullException(nameof(_emailSenderOptions));

            OnAfterInitialized();
        }
    }
}
```

Base class parameters may be passed along by providing them in the `GenerateServiceConstructor` attribute:
```
[GenerateServiceConstructor("register", "Register stuff")]
public partial class Test : Command
{
    ...
}
```

Generated code:
```
public partial class Test
{
    public Test(
        ...
    ) : base("register", "Register stuff") {
        ...
    }
}
```

This is currently implemented as a verbatim copy of the attribute's parameter list -- no parsing or type-checking occurs.

# Troubleshooting
- PROBLEM:  You receive type conversion errors after deleting an existing constructor and converting your code to use this generator.
    - SOLUTION:  The constructor parameters are generated from fields and properties in their source order.  Check that your fields are defined in the same order as your old constructor's parameters were, or update the code constructing the object to pass parameters in the new order.
- PROBLEM:  You receive the compile error `The type or namespace name 'SetsRequiredMembersAttribute' does not exist in namespace 'System.Diagnostics.CodeAnalysis'`
    - SOLUTION:  This will occur in projects using C# 10 or earlier.  Stub out the attribute in your project:
- PROBLEM:  Even when everything seems right, my constructor doesn't generate.  
    - SOLUTION:  prior to version 0.1.28, the constructor would only generate if the [GenerateServiceConstructor] attribute was the first attribute on the class.  Update your NuGet reference to 0.1.28 or later.

```
namespace System.Diagnostics.CodeAnalysis;

// stub in for now since we're not on C# 11

[AttributeUsage(AttributeTargets.Constructor, AllowMultiple = false)]
public sealed class SetsRequiredMembersAttribute : Attribute
{
}
```

# Contributing
I welcome Pull Requests for any improvement or bug fixes.  Please open an Issue for discussion if you plan on adding any features, so that we can collaborate on design.  For bug reports, a Pull Request with a failing unit test is ideal.

Thanks!
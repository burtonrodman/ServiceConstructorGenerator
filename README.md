# ServiceConstructorGenerator
A C# Source Generator that generates a constructor to initialize all readonly fields.

This reduces the amount of boiler-plate code needed when using constructor injection in ASP.Net Core projects for example.  However, this can be used with any C# project and does NOT require ASP.Net Core, or even a Dependency Injection system.

# Getting Started

1. Add the ```burtonrodman.ServiceConstructorGenerator``` NuGet package to your project.
2. Add a using for `burtonrodman.ServiceConstructorGenerator` to the top of your C# file.
3. Add a `[GenerateServiceConstructor]` attribute to your class.
4. Add the `partial` keyword on your class.
5. Optionally, add an `[InjectAsOptions]` attribute on any field that should be wrapped with IOptions.

In this example, the following constructor will be generated:

```
using burtonrodman.ServiceConstructorGenerator;

namespace MyApp
{
    [GenerateServiceConstructor]
    public partial class Test
    {
        private readonly IHttpContextAccessor _accessor;
        private readonly IWidgetRepository _widgetRepository;
        [InjectAsOptions]
        private readonly EmailSenderOptions _emailSenderOptions;
    }
}
```

Generated Code:
```
// Auto-generated code.
namespace MyApp
{
    public partial class Test
    {
        public Test(
            IHttpContextAccessor _accessor,
            IWidgetRepository _widgetRepository,
            Microsoft.Extensions.Options.IOptions<EmailSenderOptions> _emailSenderOptions
        ) {
            this._accessor = _accessor;
            this._widgetRepository = _widgetRepository;
            this._emailSenderOptions = _emailSenderOptions.Value;
        }
    }
}
```

# Troubleshooting
- PROBLEM:  You receive type conversion errors after deleting an existing constructor and converting your code to use this generator .
    - SOLUTION:  The constructor parameters are generated from fields in their source order.  Check that your fields are defined in the same order as your old constructor's parameters were, or update the code constructing the object to pass parameters in the new order.

# Contributing
I welcome Pull Requests for any improvement or bug fixes.  Please open an Issue for discussion if you plan on adding any features, so that we can collaborate on design.

Thanks!
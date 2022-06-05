# FieldInjectionGenerator
A C# Source Generator that generates a constructor to initialize all readonly fields.

This reduces the amount of boiler-plate code needed when using constructor injection in ASP.Net Core projects for example.  However, this can be used with any C# project and does NOT require ASP.Net Core, or even a Dependency Injection system.

# Getting Started

1. Add the ```burtonrodman.FieldInjectionGenerator``` NuGet package to your project.
2. Add a using for `burtonrodman.FieldInjectionGenerator` to the top of your C# file.
3. Add a `[GenerateFieldInjectionConstructor]` attribute to your class.
4. Add the `partial` keyword on your class.
5. Optionally, add an `[InjectAsOptions]` attribute on any field that should be wrapped with IOptions.

In this example, the following constructor will be generated:

```
using burtonrodman.FieldInjectionGenerator;

namespace MyApp
{
    [GenerateFieldInjectionConstructor]
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

# Future Plans
- support C# 11 required properties.
- support customizing the InjectAsOptions feature using C# 11 attribute generics and expressions

  `ex: [InjectAs<IOptions>(options => options.Value)]`

  `ex: [InjectAs<MyWrapper>(wrapper => wrapper.Object)]`

# Contributing
I welcome Pull Requests for any improvement or bug fixes.  Please open an Issue for discussion if you plan on adding any features, so that we can collaborate on design.

Thanks!
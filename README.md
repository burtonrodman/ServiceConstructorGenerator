# FieldInjectionGenerator
A C# Source Generator that generates a constructor to initialize all readonly fields.

This reduces the amount of boiler-plate code needed when using constructor injection in ASP.Net Core projects for example.  However, this can be used with any C# project and does NOT require ASP.Net Core, or even a Dependency Injection system.

# Getting Started

1. Add the ```burtonrodman.FieldInjectionGenerator``` NuGet package to your project.
2. Add a using for `burtonrodman.FieldInjectionGenerator` to the top of your C# file.
3. Add a `[GenerateFieldInjectionConstructor]` attribute to your class.
4. Add the `partial` keyword on your class.

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
            IWidgetRepository _widgetRepository
        ) {
            this._accessor = _accessor;
            this._widgetRepository = _widgetRepository;
        }
    }
}
```

# Future Plans
- support C# 11 required properties.

# Contributing
I welcome Pull Requests for any improvement or bug fixes.  Please open an Issue for discussion if you plan on adding any features, so that we can collaborate on design.

Thanks!
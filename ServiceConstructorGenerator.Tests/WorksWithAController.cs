namespace ServiceConstructorGeneratorTests;

public partial class TheServiceConstructorGenerator
{

    [Fact]
    public async void WorksWithAController()
    {
        await VerifySourceGenerator(
            """
            namespace ConsoleApp28;
            [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = false)]
            public class ApiControllerAttribute : System.Attribute { }

            [ApiController]
            [GenerateServiceConstructor]
            public partial class WorksWithAController
            {
                public readonly ITestService _bar;

            }
            """,

            CreateGeneratedSource("ConsoleApp28.WorksWithAController.g.cs",
            """
            using System;

            namespace ConsoleApp28
            {
                public partial class WorksWithAController
                {
                    partial void OnAfterInitialized();

                    public WorksWithAController(
                        ITestService _bar
                    ) {
                        this._bar = _bar ?? throw new ArgumentNullException(nameof(_bar));

                        OnAfterInitialized();
                    }
                }
            }
            """)
        );
    }
}
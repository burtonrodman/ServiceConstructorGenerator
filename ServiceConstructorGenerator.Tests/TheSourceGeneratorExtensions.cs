namespace burtonrodman;

public partial class TheServiceConstructorGenerator
{

    [Theory]
    [InlineData("testService", "testService")]
    [InlineData("_testService", "testService")]
    [InlineData("_TestService", "testService")]

    public void ToParameterName(string memberName, string parameterName)
    {
        Assert.Equal(parameterName, SourceGeneratorExtensions.ToParameterName(memberName));
    }
}
namespace burtonrodman;

public class ParameterInfo
{
    public string? TypeName { get; set; }
    public string? MemberName { get; set; }
    public (string TypeName, string InitExpression) InjectAs { get; set; }
    public int DeclarationStartingLine { get; set; }
    public bool IsRequired { get; set; }
}
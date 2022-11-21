namespace burtonrodman;

public class ParameterInfo
{
    public string? TypeName { get; set; }
    public string? MemberName { get; set; }
    public bool ShouldInjectAsOptions { get; set; }
    public int DeclarationStartingLine { get; set; }
}
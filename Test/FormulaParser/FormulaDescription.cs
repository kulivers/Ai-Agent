namespace FormulaParser;

public class FormulaDescription
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Syntax { get; set; } = string.Empty;
    public string Arguments { get; set; } = string.Empty;
    public string Result { get; set; } = string.Empty;
    public string Example { get; set; } = string.Empty;
    public string? ExampleResult { get; set; }
}

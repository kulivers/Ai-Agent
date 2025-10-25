using System.Collections.Concurrent;
using FormulaParser;
using Microsoft.SemanticKernel;
using Newtonsoft.Json;

public class FormulasParser
{
    private List<FormulaDescription?> _formulaDescriptions;
    private readonly ConcurrentDictionary<string, FormulaDescription> _formulaDescriptionsByName;

    public FormulasParser()
    {
        var formulaDescriptions = JsonConvert.DeserializeObject<List<FormulaDescription>>(File.ReadAllText(@"C:\dev\semantic-kernel\dotnet\src\Test\FormulaParser\formulas.json"));
        _formulaDescriptions = formulaDescriptions;
        var formated = this._formulaDescriptions.Select(desc => new KeyValuePair<string, FormulaDescription>(desc.Name.ToLower(), desc));
        _formulaDescriptionsByName = new ConcurrentDictionary<string, FormulaDescription>(formated);
    }

    public List<FormulaDescription?> ReadFormulas()
    {
        return _formulaDescriptions;
    }

    public FormulaDescription? ReadFormula(string formulaName)
    {
        var formatedFormula = formulaName.TrimEnd(')').TrimEnd('(').ToLower();
        _formulaDescriptionsByName.TryGetValue(formatedFormula, out var formulaDescription);
        return formulaDescription;
    }
}

using System.ComponentModel;
using FormulaParser;
using Microsoft.SemanticKernel;

namespace Test;

public class FormulasDescriptionPlugin
{
    [KernelFunction("get_formulas")]
    [Description("Gets formulas items.")]
    [return: Description("A list of formulas and its descriptions.")]
    public List<FormulaDescription?> GetFormulaDescriptions(Kernel kernel)
    {
        List<FormulaDescription?> result = new FormulasParser().ReadFormulas();
        return result;
    }

    [KernelFunction("get_formula")]
    [Description("Gets the formula by name")]
    [return: Description("Formula description with same name if found. Otherwise null.")]
    public FormulaDescription ReadFormula(Kernel kernel, string name)
    {
        var result = new FormulasParser().ReadFormula(name);
        return result;
    }
}

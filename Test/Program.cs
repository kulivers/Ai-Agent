using FormulaParser;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Newtonsoft.Json;

internal class Program
{
    public static async Task Main(string[] args)
    {
        await Test.MistralExample.Run().ConfigureAwait(false);
    }
}

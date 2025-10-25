using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;

internal class Program
{
    public static async Task Main(string[] args)
    {
        await Test.MistralExample.Run().ConfigureAwait(false);
    }
}

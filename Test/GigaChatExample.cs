// Copyright (c) Microsoft. All rights reserved.

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;

namespace Test;

public class GigaChatExample
{
    public static async Task Run()
    {
        var builder = Kernel.CreateBuilder();
        builder.AddMistralChatCompletion(
            "mistral-medium-latest",
            "IHO43sq5t332eHpjVQw5YkJxq7t4sxvQ",
            new Uri("https://api.mistral.ai/v1")
        );
        var kernel = builder.Build();

        ChatCompletionAgent agent =
            new()
            {
                Name = "SK-Agent",
                Instructions = "You are a helpful assistant.",
                Kernel = kernel,
            };

        await foreach (StreamingChatMessageContent update in agent.InvokeStreamingAsync("Write a haiku about Semantic Kernel. i need 8 raws").ConfigureAwait(false))
        {
            Console.Write(update);
        }

        await foreach (AgentResponseItem<ChatMessageContent> response in (agent.InvokeAsync("Write a haiku about Semantic Kernel. i need 8 raws.")).ConfigureAwait(false))
        {
            Console.WriteLine(response.Message);
        }
    }
}

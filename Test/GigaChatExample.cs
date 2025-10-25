// Copyright (c) Microsoft. All rights reserved.

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;

namespace Test;

public class GigaChatExample
{
    public static async Task Run()
    {
        var builder = Kernel.CreateBuilder();
        builder.AddGigachatChatCompletion(
            "deepseek/deepseek-chat-v3.1:free",
            "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",
            new Uri("https://openrouter.ai/api/v1")
        );
        var kernel = builder.Build();

        ChatCompletionAgent agent =
            new()
            {
                Name = "SK-Agent",
                Instructions = "You are a helpful assistant.",
                Kernel = kernel,
            };
        //
        // await foreach (StreamingChatMessageContent update in agent.InvokeStreamingAsync("Write a haiku about Semantic Kernel. i need 8 raws").ConfigureAwait(false))
        // {
        //     Console.Write(update);
        // }

        await foreach (AgentResponseItem<ChatMessageContent> response in (agent.InvokeAsync("Write a haiku about Semantic Kernel. i need 8 raws.")).ConfigureAwait(false))
        {
            Console.WriteLine(response.Message);
        }
    }
}

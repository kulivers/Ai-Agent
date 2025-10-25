using System.Text;
using System.Threading.Channels;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;

namespace Test;

public class MistralExample
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

        var chatService = kernel.GetRequiredService<IChatCompletionService>();
        var chatMessages = new ChatHistory();
        string? message = null;
        while (true)
        {
            try
            {
                Console.WriteLine("Prompt:");
                while (string.IsNullOrWhiteSpace(message))
                {
                    message = Console.ReadLine();
                }

                chatMessages.AddMessage(AuthorRole.User, message);

                StringBuilder sb = new StringBuilder();
                var chatCompletion = chatService.GetChatMessageContentsAsync(chatMessages, kernel: kernel);
                foreach (var response in await chatCompletion.ConfigureAwait(false))
                {
                    Console.WriteLine(response);
                    sb.Append(response.Content);
                }

                chatMessages.AddAssistantMessage(sb.ToString());
                Console.WriteLine();
                message = null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        // await foreach (StreamingChatMessageContent update in agent.InvokeStreamingAsync("Write a haiku about Semantic Kernel. i need 8 raws").ConfigureAwait(false))
        // {
        //     Console.Write(update);
        // }
    }
}

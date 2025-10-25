using System.Collections.Concurrent;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Channels;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.MistralAI;
using Newtonsoft.Json;

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

        builder.Plugins.AddFromType<FormulasDescriptionPlugin>();
        var kernel = builder.Build();

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
                var chatCompletion = chatService.GetChatMessageContentsAsync(chatMessages,
                    executionSettings: new MistralAIPromptExecutionSettings()
                    {
                        ToolCallBehavior = MistralAIToolCallBehavior.AutoInvokeKernelFunctions
                    },
                    kernel);
                foreach (var response in await chatCompletion.ConfigureAwait(false))
                {
                    Console.WriteLine(response);
                    sb.Append(response.Content);
                }

                chatMessages.AddAssistantMessage(sb.ToString());
                Console.WriteLine();
            }
            catch (Microsoft.SemanticKernel.HttpOperationException exception)
            {
                string? response = exception.ResponseContent;
                if (string.IsNullOrWhiteSpace(response))
                {
                    Console.WriteLine(exception);
                    continue;
                }

                if (response.Contains("message"))
                {
                    try
                    {
                        var error = JsonConvert.DeserializeObject<MistralError>(response)?.Message;
                        if (!string.IsNullOrWhiteSpace(error))
                        {
                            Console.WriteLine(error);
                            continue;
                        }
                    }
                    catch (Exception e)
                    {
                    }

                    Console.WriteLine(exception);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                message = null;
            }
        }

        // ChatCompletionAgent agent =
        //     new()
        //     {
        //         Name = "SK-Agent",
        //         Instructions = "You are a helpful assistant.",
        //         Kernel = kernel,
        //     };

        // await foreach (StreamingChatMessageContent update in agent.InvokeStreamingAsync("Write a haiku about Semantic Kernel. i need 8 raws").ConfigureAwait(false))
        // {
        //     Console.Write(update);
        // }
    }
}

public class MistralError
{
    [JsonPropertyName("object")]
    public string Object { get; set; } = string.Empty;

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("param")]
    public string? Param { get; set; }

    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;
}

using Microsoft.Extensions.AI;
using OllamaSharp;

// Simple test to isolate the 404 issue
var client = new OllamaApiClient(new Uri("http://localhost:11434"), "mistral:7b");

try 
{
    Console.WriteLine("Testing OllamaApiClient as IChatClient...");
    IChatClient chatClient = client;
    
    var messages = new List<ChatMessage>
    {
        new ChatMessage(ChatRole.User, "Hello")
    };
    
    var response = await chatClient.CompleteAsync(messages);
    Console.WriteLine($"Success: {response.Message.Text}");
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
    Console.WriteLine($"Stack trace: {ex.StackTrace}");
}

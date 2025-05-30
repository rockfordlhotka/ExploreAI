using Microsoft.Extensions.AI;
using OllamaSharp;

// Test to replicate the exact usage pattern from the main app
Console.WriteLine("Testing OllamaApiClient as IChatClient with history...");

var client = new OllamaApiClient(new Uri("http://localhost:11434"), "mistral:7b");

try 
{
    IChatClient chatClient = client;
    
    // Test with empty history first (like the main app)
    var history = new List<ChatMessage>();
    string userInput = "Hello";
    
    // Replicate the exact pattern from ChatService
    history.Add(new ChatMessage(ChatRole.User, userInput));
    
    Console.WriteLine("Calling GetResponseAsync with history...");
    var response = await chatClient.GetResponseAsync(history, options: null);
    Console.WriteLine($"Success: {response}");
    
    // Test what happens when we add the response to history and try again
    Console.WriteLine("\nAdding response to history...");
    history.AddMessages(response);
      Console.WriteLine($"History now has {history.Count} messages");
    foreach(var msg in history)
    {
        var textPreview = msg.Text?.Substring(0, Math.Min(50, msg.Text.Length)) ?? "";
        Console.WriteLine($"  {msg.Role}: {textPreview}...");
    }
    
    Console.WriteLine("\nTrying second message...");
    history.Add(new ChatMessage(ChatRole.User, "What's the weather like?"));
    var response2 = await chatClient.GetResponseAsync(history, options: null);
    Console.WriteLine($"Success: {response2}");
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
    Console.WriteLine($"Inner exception: {ex.InnerException?.Message}");
    Console.WriteLine($"Stack trace: {ex.StackTrace}");
}

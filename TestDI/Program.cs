using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OllamaSharp;

// Test using different OllamaApiClient configurations
Console.WriteLine("Testing with different OllamaApiClient configurations...");

var builder = Host.CreateApplicationBuilder();

// Try configuring the OllamaApiClient with custom HttpClient timeout
builder.Services.AddSingleton<IChatClient>(sp => 
{
    var client = new OllamaApiClient(new Uri("http://localhost:11434"), "mistral:7b");
    
    // Try to configure a shorter timeout
    if (client.HttpClient != null)
    {
        client.HttpClient.Timeout = TimeSpan.FromSeconds(30);
        Console.WriteLine("Set HttpClient timeout to 30 seconds");
    }
    
    return client;
});

builder.Services.AddSingleton<SimpleChatService>();

var host = builder.Build();

try 
{    
    var chatService = host.Services.GetRequiredService<SimpleChatService>();
    
    Console.WriteLine("Testing through ChatService with shorter timeout...");
    var response = await chatService.GetChatResponseAsync("Hello");
    Console.WriteLine($"Success: {response}");
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
    Console.WriteLine($"Inner exception: {ex.InnerException?.Message}");
}

// Simple ChatService for testing
public class SimpleChatService
{
    private readonly IChatClient _chatClient;

    public SimpleChatService(IChatClient chatClient)
    {
        _chatClient = chatClient;
    }

    public async Task<string> GetChatResponseAsync(string userInput, List<ChatMessage>? history = null, CancellationToken cancellationToken = default)
    {
        history ??= new List<ChatMessage>();
        history.Add(new ChatMessage(ChatRole.User, userInput));
        
        // Try with a custom cancellation token with shorter timeout
        using var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(15));
        using var combinedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);
        
        var response = await _chatClient.GetResponseAsync(history, options: null, combinedCts.Token);
        history.AddMessages(response);
        return response.ToString();
    }
}

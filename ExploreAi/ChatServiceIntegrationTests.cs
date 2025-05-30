using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.AI;
using OllamaSharp;
using System;
using System.Threading.Tasks;

namespace ExploreAi
{
    [TestClass]
    public class ChatServiceIntegrationTests
    {        [TestMethod]
        public async Task ChatService_CanConnectToOllama()
        {
            // Arrange
            var chatClient = new OllamaApiClient(new Uri("http://localhost:11434"), "mistral:7b");
            var chatService = new ChatService(chatClient);

            // Act & Assert
            try
            {
                var response = await chatService.GetChatResponseAsync("ping", null, System.Threading.CancellationToken.None);
                
                // If we get here without exception, the connection worked
                Assert.IsNotNull(response);
                Assert.IsTrue(response.Length > 0, "Response should not be empty");
                
                Console.WriteLine($"Chat response: {response}");
            }
            catch (Exception ex)
            {
                // Print the exception for debugging
                Console.WriteLine($"Chat failed with: {ex.Message}");
                
                // If it's a 404, that means the model wasn't found
                if (ex.Message.Contains("404"))
                {
                    Assert.Fail($"Model 'deepseek-r1:8b' not found. Available models should include: mistral-small:24b, deepseek-r1:8b. Error: {ex.Message}");
                }
                else
                {
                    // For other errors, we might still want to see what happened
                    Assert.Fail($"Unexpected error: {ex.Message}");
                }
            }
        }
    }
}

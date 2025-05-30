using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.AI;
// using Microsoft.Extensions.AI.Chat; // Not needed, types are in Microsoft.Extensions.AI
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
// using Moq; // Will add Moq via NuGet

namespace ExploreAi
{
    [TestClass]
    public class ChatServiceTests
    {
        [TestMethod]
        public async Task GetChatResponseAsync_ReturnsResponse()
        {
            // Arrange
            var mockClient = new Moq.Mock<IChatClient>();
            mockClient.Setup(c => c.GetResponseAsync(Moq.It.IsAny<List<Microsoft.Extensions.AI.ChatMessage>>(), null, Moq.It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new Microsoft.Extensions.AI.ChatResponse(new List<Microsoft.Extensions.AI.ChatMessage> { new Microsoft.Extensions.AI.ChatMessage(Microsoft.Extensions.AI.ChatRole.Assistant, "Hello!") })));
            var service = new ChatService(mockClient.Object);

            // Act
            var response = await service.GetChatResponseAsync("Hi");

            // Assert
            Assert.IsNotNull(response);
            Assert.AreEqual("[Assistant] Hello!", response);
        }
    }
}

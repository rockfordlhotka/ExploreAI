using Microsoft.Extensions.AI;
// using Microsoft.Extensions.AI.Chat; // Not needed, types are in Microsoft.Extensions.AI
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ExploreAi
{
    public class ChatService
    {
        private readonly IChatClient _chatClient;

        public ChatService(IChatClient chatClient)
        {
            _chatClient = chatClient;
        }

        public async Task<string> GetChatResponseAsync(string userInput, List<Microsoft.Extensions.AI.ChatMessage>? history = null, CancellationToken cancellationToken = default)
        {
            history ??= new List<Microsoft.Extensions.AI.ChatMessage>();
            history.Add(new Microsoft.Extensions.AI.ChatMessage(Microsoft.Extensions.AI.ChatRole.User, userInput));
            var response = await _chatClient.GetResponseAsync(history, options: null, cancellationToken);
            history.AddMessages(response);
            return response.ToString();
        }
    }
}

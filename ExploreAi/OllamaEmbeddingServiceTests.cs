using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace ExploreAi
{
    [TestClass]
    public class OllamaEmbeddingServiceTests
    {
        [TestMethod]
        public async Task GetEmbeddingAsync_ReturnsEmbedding()
        {
            var service = new OllamaEmbeddingService();
            var embedding = await service.GetEmbeddingAsync("hello world");
            Assert.IsNotNull(embedding);
            Assert.IsTrue(embedding.Length > 0, "Embedding should not be empty");
        }
    }
}

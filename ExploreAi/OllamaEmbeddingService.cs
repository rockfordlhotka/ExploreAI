using OllamaSharp;
using Microsoft.Extensions.AI;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace ExploreAi
{
    public class OllamaEmbeddingService
    {
        private readonly IEmbeddingGenerator<string, Embedding<float>> _embeddingGen;

        public OllamaEmbeddingService(string baseUrl = "http://localhost:11434", string model = "nomic-embed-text")
        {
            var client = new OllamaApiClient(new Uri(baseUrl), model);
            _embeddingGen = client;
        }

        public async Task<float[]> GetEmbeddingAsync(string text)
        {
            // The IEmbeddingGenerator interface exposes GenerateAsync, not GenerateEmbeddingAsync
            var result = await _embeddingGen.GenerateAsync(new[] { text }, null, System.Threading.CancellationToken.None);
            // Try to enumerate the result directly
            var embedding = result.FirstOrDefault();
            if (embedding == null)
                throw new InvalidOperationException("No embedding returned from OllamaSharp.");
            return embedding.Vector.ToArray();
        }
    }
}

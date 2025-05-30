# OllamaSharp Documentation

OllamaSharp provides .NET bindings for the Ollama API, simplifying interactions with Ollama both locally and remotely.

## Features

- Easy to use: Interact with Ollama in just a few lines of code.
- Reliable: Used by Microsoft Semantic Kernel, .NET Aspire, and Microsoft.Extensions.AI.
- Full API coverage: Supports chats, embeddings, listing models, pulling/creating models, and more.
- Real-time streaming and progress reporting.
- Multi-modality: Vision model support.

## Basic Usage

### Initializing

```csharp
var uri = new Uri("http://localhost:11434");
var ollama = new OllamaApiClient(uri);
ollama.SelectedModel = "llama3.1:8b";
```

### Listing Models

```csharp
var models = await ollama.ListLocalModelsAsync();
```

### Pulling a Model

```csharp
await foreach (var status in ollama.PullModelAsync("llama3.1:405b"))
    Console.WriteLine($"{status.Percent}% {status.Status}");
```

### Generating a Completion

```csharp
await foreach (var stream in ollama.GenerateAsync("How are you today?"))
    Console.Write(stream.Response);
```

### Interactive Chat

```csharp
var chat = new Chat(ollama);
while (true)
{
    var message = Console.ReadLine();
    await foreach (var answerToken in chat.SendAsync(message))
        Console.Write(answerToken);
}
```

## Embedding Generation

OllamaSharp implements Microsoft.Extensions.AI's `IEmbeddingGenerator<string, Embedding<float>>` interface. To use embedding generation:

```csharp
using Microsoft.Extensions.AI;
using OllamaSharp;

var ollama = new OllamaApiClient(new Uri("http://localhost:11434"), "nomic-embed-text");
IEmbeddingGenerator<string, Embedding<float>> embeddingGen = ollama;
var embedding = await embeddingGen.GenerateEmbeddingAsync("hello world", CancellationToken.None);
float[] vector = embedding.Data.ToArray();
```

- The embedding API is available via the `IEmbeddingGenerator` interface, not directly on `OllamaApiClient`.
- You must cast or assign the client to `IEmbeddingGenerator<string, Embedding<float>>`.

## Microsoft.Extensions.AI Integration

OllamaApiClient implements both `IChatClient` and `IEmbeddingGenerator` interfaces, so it can be used with Microsoft.Extensions.AI abstractions.

## Notes

- Always set the model (e.g., "nomic-embed-text") for embedding tasks.
- The embedding API is not a direct method on OllamaApiClient, but via the interface.

## References

- [GitHub](https://github.com/awaescher/OllamaSharp)
- [NuGet](https://www.nuget.org/packages/OllamaSharp)

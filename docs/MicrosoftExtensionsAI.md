# Microsoft.Extensions.AI Documentation

Microsoft.Extensions.AI provides .NET abstractions and utilities for integrating generative AI services, including chat and embedding capabilities, with a unified API and extensible middleware pipeline.

## Features

- Unified abstractions for chat and embedding (IChatClient, IEmbeddingGenerator)
- Middleware pipeline for caching, telemetry, function/tool calling, and more
- Dependency injection and builder patterns for easy integration
- Support for streaming responses and multi-modal content
- Extensible with custom middleware and client implementations

## Packages

- **Microsoft.Extensions.AI.Abstractions**: Core interfaces and types (IChatClient, IEmbeddingGenerator, etc.)
- **Microsoft.Extensions.AI**: Higher-level utilities, middleware, and dependency injection support (references Abstractions)

### Installation

```shell
dotnet add package Microsoft.Extensions.AI
# or for just the abstractions:
dotnet add package Microsoft.Extensions.AI.Abstractions
```

## Basic Usage

### Chat Client Example

```csharp
using Microsoft.Extensions.AI;
IChatClient client = new SampleChatClient(new Uri("http://coolsite.ai"), "target-ai-model");
Console.WriteLine(await client.GetResponseAsync("What is AI?"));
```

#### With Conversation History

```csharp
List<ChatMessage> history = [];
while (true)
{
    Console.Write("Q: ");
    history.Add(new(ChatRole.User, Console.ReadLine()));
    ChatResponse response = await client.GetResponseAsync(history);
    Console.WriteLine(response);
    history.AddMessages(response);
}
```

### Streaming Chat Response

```csharp
await foreach (ChatResponseUpdate update in client.GetStreamingResponseAsync("What is AI?"))
{
    Console.Write(update);
}
```

### Embedding Generation

```csharp
using Microsoft.Extensions.AI;
IEmbeddingGenerator<string, Embedding<float>> embeddingGen = ...; // e.g., from OllamaSharp
var embedding = await embeddingGen.GenerateEmbeddingAsync("hello world", CancellationToken.None);
float[] vector = embedding.Data.ToArray();
```

## Middleware & Pipelines

### Caching

```csharp
using Microsoft.Extensions.Caching.Memory;
IChatClient client = new ChatClientBuilder(sampleChatClient)
    .UseDistributedCache(new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions())))
    .Build();
```

### Telemetry

```csharp
using OpenTelemetry.Trace;
TracerProvider tracerProvider = OpenTelemetry.Sdk.CreateTracerProviderBuilder()
    .AddSource("my-source")
    .AddConsoleExporter()
    .Build();

IChatClient client = new ChatClientBuilder(sampleChatClient)
    .UseOpenTelemetry(sourceName: "my-source", configure: c => c.EnableSensitiveData = true)
    .Build();
```

### Tool/Function Calling

```csharp
using OllamaSharp;
string GetCurrentWeather() => "It's sunny";
IChatClient client = new OllamaApiClient(new Uri("http://localhost:11434"), "llama3.1");
client = ChatClientBuilderChatClientExtensions.AsBuilder(client)
    .UseFunctionInvocation()
    .Build();

ChatOptions options = new() { Tools = [AIFunctionFactory.Create(GetCurrentWeather)] };
await foreach (var update in client.GetStreamingResponseAsync("Should I wear a rain coat?", options))
{
    Console.Write(update);
}
```

### Custom Middleware

You can implement your own middleware by subclassing `DelegatingChatClient` or using the `Use` extension methods on `ChatClientBuilder`.

## Dependency Injection

```csharp
HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
builder.Services.AddChatClient(services => new SampleChatClient(new Uri("http://localhost"), "test")
    .AsBuilder()
    .UseDistributedCache()
    .UseOpenTelemetry()
    .Build(services));
```

## References

- [Microsoft Learn: Microsoft.Extensions.AI](https://learn.microsoft.com/en-us/dotnet/ai/microsoft-extensions-ai)
- [NuGet: Microsoft.Extensions.AI](https://www.nuget.org/packages/Microsoft.Extensions.AI)
- [NuGet: Microsoft.Extensions.AI.Abstractions](https://www.nuget.org/packages/Microsoft.Extensions.AI.Abstractions)

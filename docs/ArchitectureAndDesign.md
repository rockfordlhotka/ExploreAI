# ExploreAI Architecture and Design Decisions

## Project Goals

- **Build a .NET CLI app** to ingest all HTML files in the `IngestData` folder, extract and vectorize their text content using OllamaSharp embeddings, and store the results in a SQLite vector database for semantic search and chat.
- The CLI should have commands for ingestion and chat (REPL), use Spectre.Console for CLI structure, and include high-value MSTest unit tests.
- Documentation for OllamaSharp and Microsoft.Extensions.AI should be locally available for reference.

## Key Design Decisions

### 1. Technology Stack

- **.NET 8.0** for modern language features and performance.
- **Spectre.Console** for a robust, user-friendly CLI experience.
- **OllamaSharp** for local/remote LLM embedding and chat API integration.
- **Microsoft.Data.Sqlite** for lightweight, file-based vector storage.
- **MSTest** for unit and integration testing.

### 2. Service-Oriented CLI Architecture

- **HtmlIngestionService**: Extracts and cleans text from HTML files.
- **OllamaEmbeddingService**: Generates vector embeddings for text using OllamaSharp. Updated to use `GenerateAsync` as per the latest Microsoft.Extensions.AI interface.
- **VectorDbService**: Handles SQLite storage and retrieval of documents and embeddings, with serialization for float[] vectors.
- **CLI Entry (Program.cs)**: To be scaffolded with Spectre.Console, supporting modular commands for ingestion and chat.

### 3. Embedding Pipeline

- Text is extracted from HTML files, then passed to OllamaEmbeddingService for vectorization.
- Embeddings are stored in SQLite using VectorDbService, supporting efficient semantic search.
- The embedding pipeline was updated to use `GenerateAsync` (not `GenerateEmbeddingAsync`) and to enumerate the result for the embedding vector, matching the current Microsoft.Extensions.AI.Abstractions API.

### 4. Testing

- **MSTest** is used for all core services:
  - HtmlIngestionServiceTests
  - VectorDbServiceTests
  - OllamaEmbeddingServiceTests
- Tests ensure extraction, storage, and embedding logic are robust and correct.

### 5. Documentation

- Local markdown docs for:
  - OllamaSharp API usage (`docs/OllamaSharp.md`)
  - Microsoft.Extensions.AI usage (`docs/MicrosoftExtensionsAI.md`)
  - This architecture and design summary (`docs/ArchitectureAndDesign.md`)

## Progress Summary

- ✅ Project planning and requirements clarified.
- ✅ NuGet packages added: Spectre.Console, MSTest, OllamaSharp, Microsoft.Data.Sqlite.
- ✅ HtmlIngestionService and tests implemented.
- ✅ VectorDbService and tests implemented.
- ✅ OllamaEmbeddingService implemented and refactored for correct API usage (`GenerateAsync`).
- ✅ Local documentation for OllamaSharp and Microsoft.Extensions.AI created.
- ✅ Build and embedding pipeline issues resolved.
- ⏳ CLI integration and chat REPL pending.
- ⏳ Final CLI usability, error handling, and high-value integration tests pending.

## Next Steps

- Integrate all services into Spectre.Console CLI commands for ingestion and chat.
- Implement chat REPL that retrieves context from the vector DB and uses Ollama for responses.
- Add and run high-value unit and integration tests for the full pipeline.
- Finalize CLI usability, error handling, and documentation polish.

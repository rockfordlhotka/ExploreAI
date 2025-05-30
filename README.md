# ExploreAI

A .NET 8.0 command-line tool for ingesting HTML files, generating vector embeddings using Ollama, and enabling semantic search and chat over your ingested data. Built with Spectre.Console for a modern CLI experience.

## Features

- **Ingest HTML**: Extracts and cleans text from HTML files in a directory.
- **Embeddings**: Uses OllamaSharp to generate vector embeddings for each document.
- **Vector Database**: Stores documents and embeddings in a local SQLite database.
- **Semantic Chat**: REPL interface to query your ingested data using vector similarity.
- **Extensible**: Easily add new data sources or embedding models.

## Requirements

- .NET 8.0 SDK or later
- [Ollama](https://ollama.com/) running locally (default: `http://localhost:11434`)

## Ollama Setup

Before running ingestion or chat, make sure the Ollama server is running and the required model is pulled:

```pwsh
# Pull the embedding model (run in PowerShell)
ollama pull nomic-embed-text

# Start the Ollama server (if not already running)
ollama serve
```

You only need to pull the model once. The server must be running whenever you use the CLI.
- SQLite (no separate install needed; uses Microsoft.Data.Sqlite)

## Getting Started

### 1. Build the Project

```sh
# From the root directory
dotnet build ExploreAI.sln
```

### 2. Prepare Data

- Place your HTML files in the `ExploreAi/IngestData/` directory (or specify another directory with `--input`).

### 3. Ingest Data

This command extracts text from HTML files, generates embeddings, and stores them in a SQLite database.

```sh
# Default usage (uses IngestData/ and ExploreAi.db)
dotnet run --project ExploreAi -- ingest

# Specify custom input directory and database file
dotnet run --project ExploreAi -- ingest --input "MyHtmlFolder" --db "mydata.db"
```

### 4. Chat with Your Data

Start a REPL to semantically search and chat over your ingested documents.

```sh
dotnet run --project ExploreAi -- chat

# Specify a custom database file
dotnet run --project ExploreAi -- chat --db "mydata.db"
```

- Type your question and press Enter.
- The CLI will find the most relevant document and display its content.
- Type `exit` to quit the chat.

## Command Reference

### `ingest`

| Option      | Description                              | Default         |
|-------------|------------------------------------------|-----------------|
| `--input`   | Path to directory with HTML files         | `IngestData`    |
| `--db`      | Path to SQLite DB file                   | `ExploreAi.db`  |

### `chat`

| Option      | Description                              | Default         |
|-------------|------------------------------------------|-----------------|
| `--db`      | Path to SQLite DB file                   | `ExploreAi.db`  |

## Example Workflow

1. **Ingest HTML files:**

   ```sh
   dotnet run --project ExploreAi -- ingest --input "IngestData" --db "ExploreAi.db"
   ```

2. **Chat with your data:**

   ```sh
   dotnet run --project ExploreAi -- chat --db "ExploreAi.db"
   ```

## Project Structure

- `Program.cs` — CLI entry point and command definitions
- `HtmlIngestionService.cs` — Extracts and cleans text from HTML
- `OllamaEmbeddingService.cs` — Generates embeddings using OllamaSharp
- `VectorDbService.cs` — Stores and retrieves documents/embeddings in SQLite
- `IngestData/` — Default directory for HTML files

## Troubleshooting

- **Ollama not running:** Ensure Ollama is running locally at `http://localhost:11434` or update the service URL in `OllamaEmbeddingService.cs`.
- **No documents found:** Make sure you have ingested data before using the chat command.
- **Database errors:** Check file permissions and paths for the SQLite database.

## License

MIT License. See [LICENSE](LICENSE) for details.

---

*Built with [Spectre.Console](https://spectreconsole.net/), [OllamaSharp](https://github.com/ollama/ollama-sharp), and .NET 8.0.*

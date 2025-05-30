using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using ExploreAi;
using System.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;


namespace ExploreAi.Cli
{
    // Ingest Command
    public class IngestCommand : AsyncCommand<IngestCommand.Settings>
    {
        public class Settings : CommandSettings
        {
            [Description("Path to HTML files directory")]
            [CommandOption("--input <INPUT>")]
            public string Input { get; set; } = "IngestData";

            [Description("Path to SQLite DB file")]
            [CommandOption("--db <DB>")]
            public string Db { get; set; } = "ExploreAi.db";
        }

        public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
        {
            // Convert input and db paths to absolute paths
            var inputPath = Path.GetFullPath(settings.Input);
            var dbPath = Path.GetFullPath(settings.Db);

            var htmlService = new HtmlIngestionService();
            var embeddingService = new OllamaEmbeddingService();
            var vectorDb = new VectorDbService(dbPath);

            AnsiConsole.MarkupLine($"[yellow]Ingesting HTML files from:[/] {inputPath}");
            int count = 0;
            foreach (var doc in htmlService.IngestHtmlFiles(inputPath))
            {
                AnsiConsole.MarkupLine($"[blue]Processing:[/] {doc.FileName}");
                float[] embedding;
                try
                {
                    embedding = await embeddingService.GetEmbeddingAsync(doc.TextContent);
                }
                catch (Exception ex)
                {
                    AnsiConsole.MarkupLine($"[red]Embedding failed for {doc.FileName}: {ex.Message}[/]");
                    continue;
                }
                vectorDb.InsertDocument(doc.FileName, doc.TextContent, embedding);
                count++;
            }
            AnsiConsole.MarkupLine($"[green]Ingestion complete. {count} files processed.[/]");
            return 0;
        }
    }

    // Chat Command
    public class ChatCommand : AsyncCommand<ChatCommand.Settings>
    {
        public class Settings : CommandSettings
        {
            [Description("Path to SQLite DB file")]
            [CommandOption("--db <DB>")]
            public string Db { get; set; } = "ExploreAi.db";
        }

        public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
        {
            // Convert db path to absolute path
            var dbPath = Path.GetFullPath(settings.Db);
            var vectorDb = new VectorDbService(dbPath);
            var embeddingService = new OllamaEmbeddingService();

            AnsiConsole.MarkupLine("[yellow]Chat REPL started. Type 'exit' to quit.[/]");
            while (true)
            {
                var input = AnsiConsole.Ask<string>("[blue]You:[/]");
                if (input.Trim().ToLower() == "exit")
                    break;

                float[] inputEmbedding;
                try
                {
                    inputEmbedding = await embeddingService.GetEmbeddingAsync(input);
                }
                catch (Exception ex)
                {
                    AnsiConsole.MarkupLine($"[red]Embedding failed: {ex.Message}[/]");
                    continue;
                }

                // Find most similar document
                var docs = vectorDb.GetAllDocuments();
                var best = docs
                    .Select(d => new { d.FileName, d.TextContent, Score = CosineSimilarity(inputEmbedding, d.Embedding) })
                    .OrderByDescending(x => x.Score)
                    .FirstOrDefault();

                if (best == null)
                {
                    AnsiConsole.MarkupLine("[red]No documents in DB.[/]");
                    continue;
                }

                AnsiConsole.MarkupLine($"[green]Most relevant document:[/] [grey]{best.FileName}[/] (score: {best.Score:F3})");
                AnsiConsole.WriteLine(best.TextContent.Length > 500 ? best.TextContent.Substring(0, 500) + "..." : best.TextContent);
            }
            return 0;
        }

        // Cosine similarity helper
        private static float CosineSimilarity(float[] a, float[] b)
        {
            if (a.Length != b.Length) return 0f;
            float dot = 0, magA = 0, magB = 0;
            for (int i = 0; i < a.Length; i++)
            {
                dot += a[i] * b[i];
                magA += a[i] * a[i];
                magB += b[i] * b[i];
            }
            return (float)(dot / (Math.Sqrt(magA) * Math.Sqrt(magB) + 1e-8));
        }
    }

    // Main CLI setup
    public static class Program
    {
        public static int Main(string[] args)
        {
            var app = new CommandApp();
            app.Configure(config =>
            {
                config.AddCommand<IngestCommand>("ingest").WithDescription("Ingest HTML files and store embeddings in SQLite DB");
                config.AddCommand<ChatCommand>("chat").WithDescription("Chat REPL using vector DB context");
            });
            return app.Run(args);
        }
    }
}

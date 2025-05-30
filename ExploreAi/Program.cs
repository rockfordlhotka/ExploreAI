using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using ExploreAi;
using System.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.AI;
using OllamaSharp;
// using Microsoft.Extensions.AI.Chat; // Remove: not needed, types are in Microsoft.Extensions.AI


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
        private readonly ChatService _chatService;
        public ChatCommand(ChatService chatService)
        {
            _chatService = chatService;
        }

        public class Settings : CommandSettings
        {
            [Description("Path to SQLite DB file")]
            [CommandOption("--db <DB>")]
            public string Db { get; set; } = "ExploreAi.db";
        }

        public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
        {
            AnsiConsole.MarkupLine("[yellow]Chat REPL started. Type 'exit' to quit.[/]");
            var history = new List<Microsoft.Extensions.AI.ChatMessage>();
            while (true)
            {
                var input = AnsiConsole.Ask<string>("[blue]You:[/]");
                if (input.Trim().ToLower() == "exit")
                    break;

                try
                {
                    var response = await _chatService.GetChatResponseAsync(input, history);
                    AnsiConsole.MarkupLine($"[green]AI:[/] {response}");
                }
                catch (Exception ex)
                {
                    AnsiConsole.MarkupLine($"[red]Chat failed: {ex.Message}[/]");
                }
            }
            return 0;
        }

        // Helper to clean up excessive whitespace and decode HTML entities
        private static string CleanText(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return string.Empty;
            // Decode HTML entities
            var decoded = System.Net.WebUtility.HtmlDecode(input);
            // Replace 3+ newlines with 2, and 2+ with 1
            var normalized = System.Text.RegularExpressions.Regex.Replace(decoded, "\n{3,}", "\n\n");
            normalized = System.Text.RegularExpressions.Regex.Replace(normalized, "\n{2,}", "\n");
            // Remove leading/trailing whitespace and excessive spaces
            normalized = normalized.Trim();
            return normalized;
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
            var builder = Host.CreateApplicationBuilder(args);            // Register services
            builder.Services.AddSingleton<HtmlIngestionService>();
            builder.Services.AddSingleton<OllamaEmbeddingService>();
            builder.Services.AddSingleton<VectorDbService>(sp => new VectorDbService("ExploreAi.db"));
            builder.Services.AddSingleton<IChatClient>(sp => new OllamaApiClient(new Uri("http://localhost:11434"), "deepseek-r1:8b"));
            builder.Services.AddSingleton<ChatService>();

            var app = new CommandApp(new TypeRegistrar(builder.Services));
            app.Configure(config =>
            {
                config.AddCommand<IngestCommand>("ingest").WithDescription("Ingest HTML files and store embeddings in SQLite DB");
                config.AddCommand<ChatCommand>("chat").WithDescription("Chat REPL using Microsoft.Extensions.AI chat client");
            });
            return app.Run(args);
        }
    }
    // Spectre.Console TypeRegistrar for DI integration
    public class TypeRegistrar : ITypeRegistrar
    {
        private readonly IServiceCollection _builder;
        public TypeRegistrar(IServiceCollection builder) => _builder = builder;
        public ITypeResolver Build() => new TypeResolver(_builder.BuildServiceProvider());
        public void Register(Type service, Type implementation) => _builder.AddSingleton(service, implementation);
        public void RegisterInstance(Type service, object implementation) => _builder.AddSingleton(service, implementation);
        public void RegisterLazy(Type service, Func<object> factory) => _builder.AddSingleton(service, _ => factory());
    }

    public class TypeResolver : ITypeResolver, IDisposable
    {
        private readonly ServiceProvider _provider;
        public TypeResolver(ServiceProvider provider) => _provider = provider;
        public object? Resolve(Type? type) => type == null ? null : _provider.GetService(type);
        public void Dispose() => _provider.Dispose();
    }
}

using HtmlAgilityPack;
using System.Collections.Generic;
using System.IO;

namespace ExploreAi
{
    public class HtmlIngestionService
    {
        public record HtmlDocumentData(string FileName, string TextContent);

        public IEnumerable<HtmlDocumentData> IngestHtmlFiles(string folderPath)
        {
            var files = Directory.GetFiles(folderPath, "*.html");
            foreach (var file in files)
            {
                var doc = new HtmlDocument();
                doc.Load(file);
                var text = doc.DocumentNode.InnerText;
                yield return new HtmlDocumentData(Path.GetFileName(file), text);
            }
        }
    }
}

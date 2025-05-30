using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;

namespace ExploreAi
{
    [TestClass]
    public class HtmlIngestionServiceTests
    {
        [TestMethod]
        public void IngestHtmlFiles_ReturnsFilesWithText()
        {
            // Arrange
            var service = new HtmlIngestionService();
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "IngestData");

            // Act
            var results = service.IngestHtmlFiles(folderPath).ToList();

            // Assert
            Assert.IsTrue(results.Count > 0, "Should find at least one HTML file");
            foreach (var doc in results)
            {
                Assert.IsFalse(string.IsNullOrWhiteSpace(doc.TextContent), $"File {doc.FileName} should have text content");
            }
        }
    }
}

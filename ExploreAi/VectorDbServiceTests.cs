using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;

namespace ExploreAi
{
    [TestClass]
    public class VectorDbServiceTests
    {
        private string _testDbPath;
        [TestInitialize]
        public void Setup()
        {
            _testDbPath = Path.Combine(Path.GetTempPath(), $"test_vector_db_{System.Guid.NewGuid()}.sqlite");
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (File.Exists(_testDbPath))
                File.Delete(_testDbPath);
        }

        [TestMethod]
        public void InsertAndRetrieveDocument_Works()
        {
            var service = new VectorDbService(_testDbPath);
            var embedding = new float[] { 1.1f, 2.2f, 3.3f };
            service.InsertDocument("test.html", "test content", embedding);

            var docs = service.GetAllDocuments().ToList();
            Assert.AreEqual(1, docs.Count);
            Assert.AreEqual("test.html", docs[0].FileName);
            Assert.AreEqual("test content", docs[0].TextContent);
            CollectionAssert.AreEqual(embedding, docs[0].Embedding);
        }
    }
}

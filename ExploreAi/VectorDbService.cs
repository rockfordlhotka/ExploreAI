using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ExploreAi
{
    public class VectorDbService
    {
        private readonly string _dbPath;
        public VectorDbService(string dbPath)
        {
            _dbPath = dbPath;
            EnsureDb();
        }

        private void EnsureDb()
        {
            if (!File.Exists(_dbPath))
            {
                using var conn = new SqliteConnection($"Data Source={_dbPath}");
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = @"CREATE TABLE IF NOT EXISTS documents (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    file_name TEXT NOT NULL,
                    text_content TEXT NOT NULL,
                    embedding BLOB NOT NULL
                );";
                cmd.ExecuteNonQuery();
            }
        }

        public void InsertDocument(string fileName, string textContent, float[] embedding)
        {
            using var conn = new SqliteConnection($"Data Source={_dbPath}");
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO documents (file_name, text_content, embedding) VALUES (@file, @text, @embed)";
            cmd.Parameters.AddWithValue("@file", fileName);
            cmd.Parameters.AddWithValue("@text", textContent);
            cmd.Parameters.AddWithValue("@embed", FloatArrayToBytes(embedding));
            cmd.ExecuteNonQuery();
        }

        public IEnumerable<(int Id, string FileName, string TextContent, float[] Embedding)> GetAllDocuments()
        {
            using var conn = new SqliteConnection($"Data Source={_dbPath}");
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT id, file_name, text_content, embedding FROM documents";
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                yield return (
                    reader.GetInt32(0),
                    reader.GetString(1),
                    reader.GetString(2),
                    BytesToFloatArray((byte[])reader[3])
                );
            }
        }

        private static byte[] FloatArrayToBytes(float[] arr)
        {
            var bytes = new byte[arr.Length * sizeof(float)];
            Buffer.BlockCopy(arr, 0, bytes, 0, bytes.Length);
            return bytes;
        }

        private static float[] BytesToFloatArray(byte[] bytes)
        {
            var arr = new float[bytes.Length / sizeof(float)];
            Buffer.BlockCopy(bytes, 0, arr, 0, bytes.Length);
            return arr;
        }
    }
}

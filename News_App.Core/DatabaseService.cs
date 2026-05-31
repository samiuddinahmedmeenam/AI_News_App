using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace News_App
{
    public class DatabaseService
    {
        private static string GetConnectionString()
        {
            string? dbPath = Environment.GetEnvironmentVariable("NEWS_DB_PATH");

            if (string.IsNullOrWhiteSpace(dbPath))
            {
                dbPath = Path.Combine(AppContext.BaseDirectory, "news.db");
            }

            return $"Data Source={dbPath}";
        }

        public static void InitializeDatabase()
        {
            using var connection = new SqliteConnection(GetConnectionString());
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS NewsArticles (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    ProviderArticleId TEXT,
                    Title TEXT NOT NULL,
                    Description TEXT,
                    Url TEXT NOT NULL UNIQUE,
                    SourceName TEXT,
                    PublishedAt TEXT,
                    ImageUrl TEXT,
                    Language TEXT,
                    Category TEXT,  
                    FetchedAt TEXT NOT NULL,
                    Content TEXT
                );
            ";
            command.ExecuteNonQuery();


            var embeddingCommand = connection.CreateCommand();
            embeddingCommand.CommandText = @"
                CREATE TABLE IF NOT EXISTS ChunkEmbeddings (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    ChunkId INTEGER NOT NULL UNIQUE,
                    EmbeddingModel TEXT NOT NULL,
                    EmbeddingJson TEXT NOT NULL,
                    CreatedAt TEXT NOT NULL
                );
            ";
            embeddingCommand.ExecuteNonQuery();


            var chunkCommand = connection.CreateCommand();
            chunkCommand.CommandText = @"
                CREATE TABLE IF NOT EXISTS ArticleChunks (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    ArticleUrl TEXT NOT NULL,
                    ProviderArticleId TEXT,
                    ChunkIndex INTEGER NOT NULL,
                    ChunkText TEXT NOT NULL,
                    CreatedAt TEXT NOT NULL,
                    UNIQUE (ArticleUrl, ChunkIndex)
                );
            ";
            chunkCommand.ExecuteNonQuery();
        }

        public static bool SaveArticle(Article article)
        {
            using var connection = new SqliteConnection(GetConnectionString());
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT OR IGNORE INTO NewsArticles 
                (ProviderArticleId, Title, Description, Url, SourceName, PublishedAt, ImageUrl, Language, Category, FetchedAt, Content)
                VALUES ($ProviderArticleId, $Title, $Description, $Url, $SourceName, $PublishedAt, $ImageUrl, $Language, $Category, $FetchedAt, $Content);
            ";

            command.Parameters.AddWithValue("$ProviderArticleId", article.ProviderArticleId);
            command.Parameters.AddWithValue("$Title", article.Title);
            command.Parameters.AddWithValue("$Description", article.Description);
            command.Parameters.AddWithValue("$Url", article.Url);
            command.Parameters.AddWithValue("$SourceName", article.SourceName);
            command.Parameters.AddWithValue("$PublishedAt", article.PublishedAt);
            command.Parameters.AddWithValue("$ImageUrl", article.ImageUrl);
            command.Parameters.AddWithValue("$Language", article.Language);
            command.Parameters.AddWithValue("$Category", article.Category);
            command.Parameters.AddWithValue("$FetchedAt", DateTime.UtcNow.ToString("O"));
            command.Parameters.AddWithValue("$Content", article.Content);

            int rowsAffected = command.ExecuteNonQuery();

            return rowsAffected > 0;
        }

        public static SaveArticlesResult SaveArticles(List<Article> articles)
        {
            SaveArticlesResult result = new SaveArticlesResult();

            foreach (Article article in articles)
            {
                bool wasSaved = SaveArticle(article);

                if (wasSaved)
                {
                    result.SavedArticles++;
                }
                else
                {
                    result.SkippedArticles++;
                }
            }

            Console.WriteLine($"Saved new articles: {result.SavedArticles}");
            Console.WriteLine($"Skipped duplicate articles: {result.SkippedArticles}");

            return result;
        }

        public static int GetTotalArticleCount()
        {
            using var connection = new SqliteConnection(GetConnectionString());
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = "SELECT COUNT(*) FROM NewsArticles;";

            object? result = command.ExecuteScalar();


            return result == null ? 0 : Convert.ToInt32(result);
        }

        public static void SaveSelectedArticle(List<Article> articles)
        {
                       Console.WriteLine("Enter the article you are interested to save: ");
            int choice;
            bool isNumber = int.TryParse(Console.ReadLine(), out choice);
            if (isNumber && choice > 0 && choice <= articles.Count)
            {
                Article selectedArticle = articles[choice - 1];
                SaveArticle(selectedArticle);
                Console.WriteLine("Selected article saved to database.");
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter a valid article number.");
            }
        }

        public static List<Article> GetAllArticles()
        {
            List<Article> articles = new List<Article>();

            using var connection = new SqliteConnection(GetConnectionString());
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText= @"
            SELECT ProviderArticleId, Title, Description, Url, SourceName, PublishedAt, ImageUrl, Language, Category, Content
            FROM NewsArticles;
            ";

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                Article article = new Article
                {
                    ProviderArticleId = reader.IsDBNull(0) ? "" : reader.GetString(0),
                    Title = reader.IsDBNull(1) ? "" : reader.GetString(1),
                    Description = reader.IsDBNull(2) ? "" : reader.GetString(2),
                    Url = reader.IsDBNull(3) ? "" : reader.GetString(3),
                    SourceName = reader.IsDBNull(4) ? "" : reader.GetString(4),
                    PublishedAt = reader.IsDBNull(5) ? "" : reader.GetString(5),
                    ImageUrl = reader.IsDBNull(6) ? "" : reader.GetString(6),
                    Language = reader.IsDBNull(7) ? "" : reader.GetString(7),
                    Category = reader.IsDBNull(8) ? "" : reader.GetString(8),
                    Content = reader.IsDBNull(9) ? "" : reader.GetString(9)
                   
                };

                articles.Add(article);
            }
            Console.WriteLine($"Loaded {articles.Count} articles from the database.");
            return articles;
        }

        public static bool HasArticles()
        {
            using var connection = new SqliteConnection(GetConnectionString());
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = "SELECT COUNT(*) FROM NewsArticles;";

            object? result = command.ExecuteScalar();

            long count = result == null ? 0 : Convert.ToInt64(result);

            Console.WriteLine($"Total articles in database: {count}");
            return count > 0;
        }

        public static bool ChunkEmbeddingExists(int chunkId)
        {
            using var connection = new SqliteConnection(GetConnectionString());
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT COUNT(*)
                FROM ChunkEmbeddings
                WHERE ChunkId = $ChunkId;
            ";

            command.Parameters.AddWithValue("$ChunkId", chunkId);

            object? result = command.ExecuteScalar();

            long count = result == null ? 0 : Convert.ToInt64(result);

            return count > 0;
        }


        public static List<string> ChunkText(string text, int chunkSize = 300)
        {
            List<string> chunks = new List<string>();

            if (string.IsNullOrWhiteSpace(text))
            {
                return chunks;
            }

            for(int i = 0; i<text.Length; i+= chunkSize)
            {
                int length = Math.Min(chunkSize, text.Length - i);
                chunks.Add(text.Substring(i, length));
            }
            return chunks;
        }

        public static List<ArticleChunk> GetChunksWithoutEmbeddings()
        {
            List<ArticleChunk> chunks = new List<ArticleChunk>();

            using var connection = new SqliteConnection(GetConnectionString());
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT c.Id, c.ArticleUrl, c.ProviderArticleId, c.ChunkIndex, c.ChunkText, c.CreatedAt
                FROM ArticleChunks c
                LEFT JOIN ChunkEmbeddings e ON c.Id = e.ChunkId
                WHERE e.ChunkId IS NULL;
            ";

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                ArticleChunk chunk = new ArticleChunk
                {
                    Id = reader.GetInt32(0),
                    ArticleUrl = reader.IsDBNull(1) ? "" : reader.GetString(1),
                    ProviderArticleId = reader.IsDBNull(2) ? "" : reader.GetString(2),
                    ChunkIndex = reader.GetInt32(3),
                    ChunkText = reader.IsDBNull(4) ? "" : reader.GetString(4),
                    CreatedAt = reader.IsDBNull(5) ? "" : reader.GetString(5)
                };

                chunks.Add(chunk);
            }

            return chunks;
        }

        public static string BuildChunkSourceText(Article article)
        {
            if(!string.IsNullOrWhiteSpace(article.Content) && article.Content != "ONLY AVAILABLE IN PAID PLANS")
            {
                return article.Content;
            }

            return $"{ article.Title}\n{ article.Description}";
        }

        public static void SaveArticleChunks(Article article)
        {
            string sourceText = BuildChunkSourceText(article);
            List<string> chunks = ChunkText(sourceText);

            using var connection = new SqliteConnection(GetConnectionString());
            connection.Open();

            for(int i = 0; i<chunks.Count; i++)
            {
                var command = connection.CreateCommand();
                command.CommandText = @"
                    INSERT OR IGNORE INTO ArticleChunks
                    (ArticleUrl, ProviderArticleId, ChunkIndex, ChunkText, CreatedAt)
                    VALUES
                    ($ArticleUrl, $ProviderArticleId, $ChunkIndex, $ChunkText, $CreatedAt);
                    ";

                command.Parameters.AddWithValue("$ArticleUrl", article.Url);
                command.Parameters.AddWithValue("$ProviderArticleId", article.ProviderArticleId);
                command.Parameters.AddWithValue("$ChunkIndex", i);
                command.Parameters.AddWithValue("$ChunkText", chunks[i]);
                command.Parameters.AddWithValue("$CreatedAt", DateTime.UtcNow.ToString("O"));

                command.ExecuteNonQuery();
            }
        }

        public static void SaveArticleChunks(List<Article> articles)
        {
            foreach(var article in articles)
            {
                SaveArticleChunks(article);
            }
            Console.WriteLine($"Saved chunks for {articles.Count} articles to the database.");
        }

        public static List<ArticleChunk> GetAllChunks()
        {
            List<ArticleChunk> chunks = new List<ArticleChunk>();

            using var connection = new SqliteConnection(GetConnectionString());
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
        SELECT Id, ArticleUrl, ProviderArticleId, ChunkIndex, ChunkText, CreatedAt
        FROM ArticleChunks;
    ";

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                ArticleChunk chunk = new ArticleChunk
                {
                    Id = reader.GetInt32(0),
                    ArticleUrl = reader.IsDBNull(1) ? "" : reader.GetString(1),
                    ProviderArticleId = reader.IsDBNull(2) ? "" : reader.GetString(2),
                    ChunkIndex = reader.GetInt32(3),
                    ChunkText = reader.IsDBNull(4) ? "" : reader.GetString(4),
                    CreatedAt = reader.IsDBNull(5) ? "" : reader.GetString(5)
                };

                chunks.Add(chunk);
            }

            return chunks;
        }

        public static void SaveChunkEmbedding(int chunkId, List<float> embedding, string model = "text-embedding-3-small")
        {
            using var connection = new SqliteConnection(GetConnectionString());
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
        INSERT OR REPLACE INTO ChunkEmbeddings
        (ChunkId, EmbeddingModel, EmbeddingJson, CreatedAt)
        VALUES
        ($ChunkId, $EmbeddingModel, $EmbeddingJson, $CreatedAt);
    ";

            command.Parameters.AddWithValue("$ChunkId", chunkId);
            command.Parameters.AddWithValue("$EmbeddingModel", model);
            command.Parameters.AddWithValue("$EmbeddingJson", JsonSerializer.Serialize(embedding));
            command.Parameters.AddWithValue("$CreatedAt", DateTime.UtcNow.ToString("O"));

            command.ExecuteNonQuery();
        }


        public static Dictionary<int, List<float>> GetAllChunkEmbeddings()
        {
            Dictionary<int, List<float>> embeddings = new Dictionary<int, List<float>>();

            using var connection = new SqliteConnection(GetConnectionString());
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
        SELECT ChunkId, EmbeddingJson
        FROM ChunkEmbeddings;
    ";

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                int chunkId = reader.GetInt32(0);
                string embeddingJson = reader.GetString(1);

                List<float>? embedding = JsonSerializer.Deserialize<List<float>>(embeddingJson);

                if (embedding != null)
                {
                    embeddings[chunkId] = embedding;
                }
            }

            return embeddings;
        }


    }
}

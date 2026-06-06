using Npgsql;
using System.Text.Json;

namespace News_App
{
    public class PostgresDatabaseService
    {
        private static string GetConnectionString()
        {
            string? connectionString = Environment.GetEnvironmentVariable("POSTGRES_CONNECTION_STRING");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new Exception("POSTGRES_CONNECTION_STRING environment variable is not set.");
            }

            return connectionString;
        }

        private static object DbValue(string? value)
        {
            return string.IsNullOrWhiteSpace(value) ? DBNull.Value : value;
        }

        public static async Task TestConnection()
        {
            await using var connection = new NpgsqlConnection(GetConnectionString());
            await connection.OpenAsync();

            Console.WriteLine("PostgreSQL connection successful.");
        }

        public static async Task InitializeDatabase()
        {
            await using var connection = new NpgsqlConnection(GetConnectionString());
            await connection.OpenAsync();

            await using var articleCommand = connection.CreateCommand();
            articleCommand.CommandText = @"
                CREATE TABLE IF NOT EXISTS NewsArticles (
                    Id SERIAL PRIMARY KEY,
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
            await articleCommand.ExecuteNonQueryAsync();

            await using var chunkCommand = connection.CreateCommand();
            chunkCommand.CommandText = @"
                CREATE TABLE IF NOT EXISTS ArticleChunks (
                    Id SERIAL PRIMARY KEY,
                    ArticleUrl TEXT NOT NULL,
                    ProviderArticleId TEXT,
                    ChunkIndex INTEGER NOT NULL,
                    ChunkText TEXT NOT NULL,
                    CreatedAt TEXT NOT NULL,
                    UNIQUE (ArticleUrl, ChunkIndex)
                );
            ";
            await chunkCommand.ExecuteNonQueryAsync();

            await using var embeddingCommand = connection.CreateCommand();
            embeddingCommand.CommandText = @"
                CREATE TABLE IF NOT EXISTS ChunkEmbeddings (
                    Id SERIAL PRIMARY KEY,
                    ChunkId INTEGER NOT NULL UNIQUE,
                    EmbeddingModel TEXT NOT NULL,
                    EmbeddingJson TEXT NOT NULL,
                    CreatedAt TEXT NOT NULL
                );
            ";
            await embeddingCommand.ExecuteNonQueryAsync();

            Console.WriteLine("PostgreSQL tables initialized.");
        }

        public static async Task<bool> SaveArticle(Article article)
        {
            await using var connection = new NpgsqlConnection(GetConnectionString());
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO NewsArticles
                (ProviderArticleId, Title, Description, Url, SourceName, PublishedAt, ImageUrl, Language, Category, FetchedAt, Content)
                VALUES
                (@ProviderArticleId, @Title, @Description, @Url, @SourceName, @PublishedAt, @ImageUrl, @Language, @Category, @FetchedAt, @Content)
                ON CONFLICT (Url) DO NOTHING;
            ";

            command.Parameters.AddWithValue("@ProviderArticleId", DbValue(article.ProviderArticleId));
            command.Parameters.AddWithValue("@Title", article.Title);
            command.Parameters.AddWithValue("@Description", DbValue(article.Description));
            command.Parameters.AddWithValue("@Url", article.Url);
            command.Parameters.AddWithValue("@SourceName", DbValue(article.SourceName));
            command.Parameters.AddWithValue("@PublishedAt", DbValue(article.PublishedAt));
            command.Parameters.AddWithValue("@ImageUrl", DbValue(article.ImageUrl));
            command.Parameters.AddWithValue("@Language", DbValue(article.Language));
            command.Parameters.AddWithValue("@Category", DbValue(article.Category));
            command.Parameters.AddWithValue("@FetchedAt", DateTime.UtcNow.ToString("O"));
            command.Parameters.AddWithValue("@Content", DbValue(article.Content));

            int rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }

        public static async Task<SaveArticlesResult> SaveArticles(List<Article> articles)
        {
            SaveArticlesResult result = new SaveArticlesResult();

            foreach (Article article in articles)
            {
                bool wasSaved = await SaveArticle(article);

                if (wasSaved)
                {
                    result.SavedArticles++;
                }
                else
                {
                    result.SkippedArticles++;
                }
            }

            Console.WriteLine($"PostgreSQL saved new articles: {result.SavedArticles}");
            Console.WriteLine($"PostgreSQL skipped duplicate articles: {result.SkippedArticles}");

            return result;
        }

        public static async Task<int> GetTotalArticleCount()
        {
            await using var connection = new NpgsqlConnection(GetConnectionString());
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = "SELECT COUNT(*) FROM NewsArticles;";

            object? result = await command.ExecuteScalarAsync();

            return result == null ? 0 : Convert.ToInt32(result);
        }

        public static async Task<List<Article>> GetAllArticles()
        {
            List<Article> articles = new List<Article>();

            await using var connection = new NpgsqlConnection(GetConnectionString());
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT ProviderArticleId, Title, Description, Url, SourceName, PublishedAt, ImageUrl, Language, Category, Content
                FROM NewsArticles
                ORDER BY PublishedAt DESC NULLS LAST;
            ";

            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
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

            Console.WriteLine($"Loaded {articles.Count} articles from PostgreSQL.");
            return articles;
        }

        public static async Task<bool> HasArticles()
        {
            int count = await GetTotalArticleCount();

            Console.WriteLine($"Total PostgreSQL articles: {count}");

            return count > 0;
        }

        public static List<string> ChunkText(string text, int chunkSize = 300)
        {
            List<string> chunks = new List<string>();

            if (string.IsNullOrWhiteSpace(text))
            {
                return chunks;
            }

            for (int i = 0; i < text.Length; i += chunkSize)
            {
                int length = Math.Min(chunkSize, text.Length - i);
                chunks.Add(text.Substring(i, length));
            }

            return chunks;
        }

        public static string BuildChunkSourceText(Article article)
        {
            if (!string.IsNullOrWhiteSpace(article.Content) && article.Content != "ONLY AVAILABLE IN PAID PLANS")
            {
                return article.Content;
            }

            return $"{article.Title}\n{article.Description}";
        }

        public static async Task SaveArticleChunks(Article article)
        {
            string sourceText = BuildChunkSourceText(article);
            List<string> chunks = ChunkText(sourceText);

            await using var connection = new NpgsqlConnection(GetConnectionString());
            await connection.OpenAsync();

            for (int i = 0; i < chunks.Count; i++)
            {
                await using var command = connection.CreateCommand();
                command.CommandText = @"
                    INSERT INTO ArticleChunks
                    (ArticleUrl, ProviderArticleId, ChunkIndex, ChunkText, CreatedAt)
                    VALUES
                    (@ArticleUrl, @ProviderArticleId, @ChunkIndex, @ChunkText, @CreatedAt)
                    ON CONFLICT (ArticleUrl, ChunkIndex) DO NOTHING;
                ";

                command.Parameters.AddWithValue("@ArticleUrl", article.Url);
                command.Parameters.AddWithValue("@ProviderArticleId", DbValue(article.ProviderArticleId));
                command.Parameters.AddWithValue("@ChunkIndex", i);
                command.Parameters.AddWithValue("@ChunkText", chunks[i]);
                command.Parameters.AddWithValue("@CreatedAt", DateTime.UtcNow.ToString("O"));

                await command.ExecuteNonQueryAsync();
            }
        }

        public static async Task SaveArticleChunks(List<Article> articles)
        {
            foreach (Article article in articles)
            {
                await SaveArticleChunks(article);
            }

            Console.WriteLine($"Saved chunks for {articles.Count} articles to PostgreSQL.");
        }

        public static async Task<List<ArticleChunk>> GetAllChunks()
        {
            List<ArticleChunk> chunks = new List<ArticleChunk>();

            await using var connection = new NpgsqlConnection(GetConnectionString());
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT Id, ArticleUrl, ProviderArticleId, ChunkIndex, ChunkText, CreatedAt
                FROM ArticleChunks;
            ";

            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
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

        public static async Task<List<ArticleChunk>> GetChunksWithoutEmbeddings()
        {
            List<ArticleChunk> chunks = new List<ArticleChunk>();

            await using var connection = new NpgsqlConnection(GetConnectionString());
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT c.Id, c.ArticleUrl, c.ProviderArticleId, c.ChunkIndex, c.ChunkText, c.CreatedAt
                FROM ArticleChunks c
                LEFT JOIN ChunkEmbeddings e ON c.Id = e.ChunkId
                WHERE e.ChunkId IS NULL;
            ";

            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
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

        public static async Task<bool> ChunkEmbeddingExists(int chunkId)
        {
            await using var connection = new NpgsqlConnection(GetConnectionString());
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT COUNT(*)
                FROM ChunkEmbeddings
                WHERE ChunkId = @ChunkId;
            ";

            command.Parameters.AddWithValue("@ChunkId", chunkId);

            object? result = await command.ExecuteScalarAsync();

            long count = result == null ? 0 : Convert.ToInt64(result);

            return count > 0;
        }

        public static async Task SaveChunkEmbedding(int chunkId, List<float> embedding, string model = "text-embedding-3-small")
        {
            await using var connection = new NpgsqlConnection(GetConnectionString());
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO ChunkEmbeddings
                (ChunkId, EmbeddingModel, EmbeddingJson, CreatedAt)
                VALUES
                (@ChunkId, @EmbeddingModel, @EmbeddingJson, @CreatedAt)
                ON CONFLICT (ChunkId)
                DO UPDATE SET
                    EmbeddingModel = EXCLUDED.EmbeddingModel,
                    EmbeddingJson = EXCLUDED.EmbeddingJson,
                    CreatedAt = EXCLUDED.CreatedAt;
            ";

            command.Parameters.AddWithValue("@ChunkId", chunkId);
            command.Parameters.AddWithValue("@EmbeddingModel", model);
            command.Parameters.AddWithValue("@EmbeddingJson", JsonSerializer.Serialize(embedding));
            command.Parameters.AddWithValue("@CreatedAt", DateTime.UtcNow.ToString("O"));

            await command.ExecuteNonQueryAsync();
        }

        public static async Task<Dictionary<int, List<float>>> GetAllChunkEmbeddings()
        {
            Dictionary<int, List<float>> embeddings = new Dictionary<int, List<float>>();

            await using var connection = new NpgsqlConnection(GetConnectionString());
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT ChunkId, EmbeddingJson
                FROM ChunkEmbeddings;
            ";

            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
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
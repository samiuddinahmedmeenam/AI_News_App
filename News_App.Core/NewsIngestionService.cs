namespace News_App
{
    public class NewsIngestionService
    {
        public static async Task<NewsIngestionResult> RefreshNews()
        {
            DateTime startedAt = DateTime.UtcNow;

            DatabaseService.InitializeDatabase();

            // 1. Fetch latest articles
            List<Article> articles = await NewsServices.GetTopNews();

            // 2. Save articles
            SaveArticlesResult saveResult = DatabaseService.SaveArticles(articles);

            // 3. Save chunks
            DatabaseService.SaveArticleChunks(articles);


            // 4. Get total article count after save
            int totalArticles = DatabaseService.GetTotalArticleCount();
            Console.WriteLine($"TOTAL ARTICLES AFTER SAVE: {totalArticles}");

            // 5. Load chunk counts
            List<ArticleChunk> allChunks = DatabaseService.GetAllChunks();
            List<ArticleChunk> chunksWithoutEmbeddings = DatabaseService.GetChunksWithoutEmbeddings();

            int newEmbeddingCount = 0;
            int skippedEmbeddingCount = allChunks.Count - chunksWithoutEmbeddings.Count;

            // 6. Create embeddings only for chunks that do not already have embeddings
            foreach (ArticleChunk chunk in chunksWithoutEmbeddings)
            {
                List<float> embedding = await EmbeddingService.GetEmbedding(chunk.ChunkText);
                DatabaseService.SaveChunkEmbedding(chunk.Id, embedding);

                newEmbeddingCount++;
            }

            DateTime finishedAt = DateTime.UtcNow;

            // 7. Return result summary
            return new NewsIngestionResult
            {
                Message = "News refresh complete.",
                FetchedArticles = articles.Count,
                SavedArticles = saveResult.SavedArticles,
                SkippedArticles = saveResult.SkippedArticles,
                TotalArticles = totalArticles,
                TotalChunks = allChunks.Count,
                NewEmbeddings = newEmbeddingCount,
                SkippedEmbeddings = skippedEmbeddingCount,
                StartedAt = startedAt,
                FinishedAt = finishedAt,
                DurationSeconds = (finishedAt - startedAt).TotalSeconds,
            };
        }
    }
}
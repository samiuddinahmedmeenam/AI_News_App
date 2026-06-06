namespace News_App
{
    public class NewsIngestionService
    {
        public static async Task<NewsIngestionResult> RefreshNews()
        {
            DateTime startedAt = DateTime.UtcNow;

            await PostgresDatabaseService.InitializeDatabase();

            // 1. Fetch latest articles
            List<Article> articles = await NewsServices.GetTopNews();

            // 2. Save articles
            SaveArticlesResult saveResult = await PostgresDatabaseService.SaveArticles(articles);

            // 3. Save chunks
            await PostgresDatabaseService.SaveArticleChunks(articles);

            // 4. Get total article count after save
            int totalArticles = await PostgresDatabaseService.GetTotalArticleCount();

            // 5. Load chunks from database
            List<ArticleChunk> allChunks = await PostgresDatabaseService.GetAllChunks();
            List<ArticleChunk> chunksWithoutEmbeddings = await PostgresDatabaseService.GetChunksWithoutEmbeddings();

            int newEmbeddingCount = 0;
            int skippedEmbeddingCount = allChunks.Count - chunksWithoutEmbeddings.Count;

            // 6. Create embeddings only for chunks that do not already have embeddings
            foreach (ArticleChunk chunk in chunksWithoutEmbeddings)
            {
                List<float> embedding = await EmbeddingService.GetEmbedding(chunk.ChunkText);
                await PostgresDatabaseService.SaveChunkEmbedding(chunk.Id, embedding);

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
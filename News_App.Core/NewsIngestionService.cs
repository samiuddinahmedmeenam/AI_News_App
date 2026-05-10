namespace News_App
{
    public class NewsIngestionService
    {
        public static async Task<NewsIngestionResult> RefreshNews()
        {
            DatabaseService.InitializeDatabase();

            // 1. Fetch latest articles
            List<Article> articles = await NewsServices.GetTopNews();

            // 2. Save articles
            DatabaseService.SaveArticles(articles);

            // 3. Save chunks
            DatabaseService.SaveArticleChunks(articles);

            // 4. Load all chunks from database
            List<ArticleChunk> allChunks = DatabaseService.GetAllChunks();

            int newEmbeddingCount = 0;
            int skippedEmbeddingCount = 0;

            // 5. Create embeddings only for chunks that do not already have embeddings
            foreach (ArticleChunk chunk in allChunks)
            {
                if (DatabaseService.ChunkEmbeddingExists(chunk.Id))
                {
                    skippedEmbeddingCount++;
                    continue;
                }

                List<float> embedding = await EmbeddingService.GetEmbedding(chunk.ChunkText);
                DatabaseService.SaveChunkEmbedding(chunk.Id, embedding);
                newEmbeddingCount++;
            }

            // 6. Return result summary
            return new NewsIngestionResult
            {
                Message = "News refresh complete.",
                FetchedArticles = articles.Count,
                TotalChunks = allChunks.Count,
                NewEmbeddings = newEmbeddingCount,
                SkippedEmbeddings = skippedEmbeddingCount
            };
        }
    }
}
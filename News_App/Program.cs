using News_App;

Console.WriteLine("Starting app...");

// 1. Initialize database
DatabaseService.InitializeDatabase();

List<Article> articles;

// 2. Check if database already has articles
if (DatabaseService.HasArticles())
{
    Console.WriteLine("Articles already exist in the database. Loading from database...");
    articles = DatabaseService.GetAllArticles();
}
else
{
    Console.WriteLine("Database is empty. Fetching fresh news from API...");

    // Fetch fresh articles
    articles = await NewsServices.GetTopNews();

    // Save articles
    DatabaseService.SaveArticles(articles);
    Console.WriteLine($"{articles.Count} articles saved to the database.");

    // Save chunks
    DatabaseService.SaveArticleChunks(articles);
    Console.WriteLine("Article chunks saved to the database.");

    // Generate embeddings only for chunks that do not already have embeddings
    List<ArticleChunk> chunksToEmbed = DatabaseService.GetAllChunks();

    int newEmbeddingCount = 0;
    int skippedEmbeddingCount = 0;

    foreach (var chunk in chunksToEmbed)
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

    Console.WriteLine($"New embeddings saved: {newEmbeddingCount}");
    Console.WriteLine($"Embeddings skipped already existed: {skippedEmbeddingCount}");
}

// 3. Display loaded articles
Console.WriteLine("\nArticles:");
DisplayServices.DisplayMultipleArticles(articles);

// 4. Ask user a question
Console.WriteLine("\nEnter your question:");
string question = Console.ReadLine() ?? "";

// 5. Generate embedding for the question
List<float> questionEmbedding = await EmbeddingService.GetEmbedding(question);

// 6. Load chunks + embeddings from DB
List<ArticleChunk> allChunks = DatabaseService.GetAllChunks();
Dictionary<int, List<float>> chunkEmbeddings = DatabaseService.GetAllChunkEmbeddings();

// 7. Retrieve top relevant chunks using semantic similarity
List<ArticleChunk> relevantChunks =
    RetrievalService.RetrieveRelevantChunksSemantic(questionEmbedding, allChunks, chunkEmbeddings);

// 8. Show retrieved chunks
Console.WriteLine("\nTop Relevant Article Chunks:");
for (int i = 0; i < relevantChunks.Count; i++)
{
    Console.WriteLine($"\nChunk {i + 1}:");
    Console.WriteLine(relevantChunks[i].ChunkText);
}

// 9. Build context
string context = RetrievalService.BuildContextFromChunks(relevantChunks);
Console.WriteLine("\nContext sent to AI:");
Console.WriteLine(context);

// 10. Ask OpenAI to answer using only retrieved context
string ragAnswer = await AiSummaryServices.AnswerWithContext(question, context);

// 11. Show final answer
Console.WriteLine("\nRAG Answer:");
Console.WriteLine(ragAnswer);
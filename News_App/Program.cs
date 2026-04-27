using News_App;

Console.WriteLine("Fetching news...");

DatabaseService.InitializeDatabase();

List<Article> articles = await NewsServices.GetTopNews();


DatabaseService.SaveArticles(articles);
Console.WriteLine(articles.Count + " articles saved to the database.");


DatabaseService.SaveArticleChunks(articles);
Console.WriteLine("\nArticle chunks saved to the database.");

Console.WriteLine("\nEnter your question:");
string question = Console.ReadLine() ?? "";

List<ArticleChunk> allChunks = DatabaseService.GetAllChunks();
List<ArticleChunk> relevantChunks = RetrievalService.RetrieveRelevantChunks(question, allChunks);

Console.WriteLine("\nTop Relevant Article Chunks:");
for (int i = 0; i < relevantChunks.Count; i++)
{
    Console.WriteLine($"\nChunk {i + 1}:");
    Console.WriteLine(relevantChunks[i].ChunkText);
}

string context = RetrievalService.BuildContextFromChunks(relevantChunks);

string ragAnswer = await AiSummaryServices.AnswerWithContext(question, context);

Console.WriteLine("\nRAG Answer:");
Console.WriteLine(ragAnswer);
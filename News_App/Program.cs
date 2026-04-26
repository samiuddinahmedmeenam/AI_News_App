using News_App;

Console.WriteLine("Fetching news...");

DatabaseService.InitializeDatabase();

List<Article> articles;

if (DatabaseService.ArticleExists())
{
    Console.WriteLine("Articles found in the database. Loading from database...");
    articles = DatabaseService.GetAllArticles();
}
else
{
    Console.WriteLine("No articles in the database. Fetching from API...");
    articles = await NewsServices.GetTopNews();
    DatabaseService.SaveArticles(articles);
    Console.WriteLine("Articles saved to the database.");
}

Console.WriteLine("\nArticles:");
DisplayServices.DisplaySingleArticleMetadata(articles);


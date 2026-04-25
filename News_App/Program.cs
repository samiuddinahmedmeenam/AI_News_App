using News_App;

Console.WriteLine("Fetching news...");

DatabaseService.InitializeDatabase();

List<Article> articles = await NewsServices.GetTopNews();

DatabaseService.SaveArticles(articles);
Console.WriteLine("Articles saved to database.");

List<Article> articles1 = await NewsServices.GetTopNews();

Console.WriteLine("Select the news to be saved int the database: ");

DisplayServices.DisplayMultipleArticles(articles1);

DisplayServices.DisplaySelectedArticle(articles1);

DatabaseService.SaveSelectedArticle(articles1);

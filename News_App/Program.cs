using News_App;

Console.WriteLine("Fetching news...");

DatabaseService.InitializeDatabase();

List<Article> articles = await NewsServices.GetTopNews();

DatabaseService.SaveArticles(articles);
Console.WriteLine("Articles saved to database.");

List<Article> savedArticles = DatabaseService.GetAllArticles();

Console.WriteLine("\nArticles read from database:");
DisplayServices.DisplayMultipleArticles(savedArticles);
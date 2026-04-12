// See https://aka.ms/new-console-template for more information
Console.WriteLine("Fetching news...");

Article article = new Article();

article.Title = "Breaking News: C# 12 Released!";
article.Description = "The latest version of C# has been released, bringing new features and improvements to the language.";
article.Url = "https://www.example.com/news/csharp-12-released";

Console.WriteLine($"Title: {article.Title}");
Console.WriteLine($"Description: {article.Description}");
Console.WriteLine($"URL: {article.Url}");

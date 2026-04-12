// See https://aka.ms/new-console-template for more information
Console.WriteLine("Fetching news...");

// Define the Article class
Article article = new Article();
// Set properties of the article
article.Title = "Breaking News: C# 12 Released!";
article.Description = "The latest version of C# has been released, bringing new features and improvements to the language.";
article.Url = "https://www.example.com/news/csharp-12-released";
// Display the article information
Console.WriteLine($"Title: {article.Title}");
Console.WriteLine($"Description: {article.Description}");
Console.WriteLine($"URL: {article.Url}");


// Create a list of articles
List<Article> articles = new List<Article>();
// Add the first article to the list
articles.Add(new Article
{
    Title = "Tech Giants Announce New Collaboration",
    Description = "Several major tech companies have announced a new collaboration to develop innovative technologies.",
    Url = "https://www.example.com/news/tech-giants-collaboration"
});
// Add the second article to the list
articles.Add(new Article
{
    Title = "Global Economy Shows Signs of Recovery",
    Description = "Economic indicators suggest that the global economy is showing signs of recovery after a challenging period.",
    Url = "https://www.example.com/news/global-economy-recovery"
});
// Print the articles
foreach (var articleItem in articles)
{
    Console.WriteLine($"Title: {articleItem.Title}");
    Console.WriteLine($"Description: {articleItem.Description}");
    Console.WriteLine($"URL: {articleItem.Url}");
}

int i = 1;

foreach (var articleItem in articles)
{
    Console.WriteLine($"News {i++}:");
    Console.WriteLine($"Title: {articleItem.Title}");
    Console.WriteLine($"Description: {articleItem.Description}");
    Console.WriteLine($"URL: {articleItem.Url}");
}


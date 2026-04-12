using News_App;
using System.IO;

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Fetching news...");

List<Article> articles = NewsServices.GetTopNews();

for(int i = 0; i<articles.Count; i++)
{
    Console.WriteLine($"Top news {i + 1}\nTitle: {articles[i].Title}\nDescription: {articles[i].Description}\nURL: {articles[i].Url}");
}


Console.WriteLine(Path.GetFullPath("articles.txt"));

// Save the article information to a text file using FileServices class
//FileServices.SaveArticleToFile(article);
FileServices.SaveMultipleArticlesToFile(articles);

// display the articles using the DisplayServices.cs
//DisplayServices.DisplaySingleArticle(article);
DisplayServices.DisplaySelectedArticles(articles);
DisplayServices.DisplayMultipleArticles(articles);




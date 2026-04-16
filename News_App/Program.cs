using News_App;
using System.IO;

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Fetching news...");

List<Article> articles = await NewsServices.GetTopNews();

for(int i = 0; i<articles.Count; i++)
{
    Console.WriteLine($"Top news {i + 1}\nTitle: {articles[i].Title}\nDescription: {articles[i].Description}\nURL: {articles[i].Url}");
}


// Console.WriteLine(Path.GetFullPath("articles.txt"));

// Save the article information to a text file using FileServices class
// FileServices.SaveArticleToFile(article);
File.Delete("articles.txt");
File.Delete("article.txt");
// FileServices.SaveMultipleArticlesToFile(articles);
FileServices.SaveSelectedArticleToFile(articles);
Console.WriteLine(Path.GetFullPath("articles.txt"));

// display the articles using the DisplayServices.cs
//DisplayServices.DisplaySingleArticle(article);
DisplayServices.DisplaySelectedArticle(articles);
// DisplayServices.DisplayMultipleArticles(articles);


// string JSON = await NewsServices.CallAPI();
// Console.WriteLine($"JSON {JSON}");

// get the AI summary
int choice = int.Parse(Console.ReadLine());
Article selectedArticle = articles[choice - 1];
string summary = await AiSummaryServices.GetSummary(selectedArticle);
Console.WriteLine(summary);

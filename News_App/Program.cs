using News_App;

Console.WriteLine("Fetching news...");

List<Article> articles = await NewsServices.GetTopNews();

for (int i = 0; i < articles.Count; i++)
{
    Console.WriteLine($"Top news {i + 1}");
    Console.WriteLine($"Title: {articles[i].Title}");
    Console.WriteLine($"Description: {articles[i].Description}");
    Console.WriteLine($"URL: {articles[i].Url}");
    Console.WriteLine();
}

Console.WriteLine("Enter the article you're interested in:");

int choice;
while (!int.TryParse(Console.ReadLine(), out choice) || choice < 1 || choice > articles.Count)
{
    Console.WriteLine($"Invalid input. Enter a number between 1 and {articles.Count}:");
}

Article selectedArticle = articles[choice - 1];

string summary = await AiSummaryServices.GetSummary(selectedArticle);

Console.WriteLine("\nAI Summary:");
Console.WriteLine(summary);
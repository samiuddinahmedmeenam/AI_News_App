using System.IO;

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
// print the articles with numbers
int i = 1;
foreach (var articleItem in articles)
{
    Console.WriteLine($"News {i++}:");
    Console.WriteLine($"Title: {articleItem.Title}");
    Console.WriteLine($"Description: {articleItem.Description}");
    Console.WriteLine($"URL: {articleItem.Url}");
}
// ask for user choice
// Console.WriteLine("Enter the Article number you are interested in:");
//int choice = int.Parse(Console.ReadLine();
// confirm user choice
// Console.WriteLine($"You selected: {choice} which is '{articles[choice - 1].Title}'");
// Console.WriteLine($"Press 1 do display News description, Press 2 to select another one:");
// int choice2 = int.Parse(Console.ReadLine());

// get user choice
Console.WriteLine($"Select the article number you are interested in:");
// int choice = int.Parse(Console.ReadLine());
int choice;
bool isNumber = int.TryParse(Console.ReadLine(), out choice);

// validate user choice
while (true)
{
    // check if user inputed a number to avoid strings
    if (isNumber)
    {
        // check if the number is within the range of available articles
        if (choice >= 1 && choice <= articles.Count)
        {
            Console.WriteLine($"You selected: {choice} which is '{articles[choice - 1].Title}'");
            Console.WriteLine($"Press 1 to display News description, Press 2 to select another one:");
            int choice2 = int.Parse(Console.ReadLine());
            if (choice2 == 1)
            {
                Console.WriteLine($"Description: {articles[choice - 1].Description}");
                break;
            }
            else if (choice2 == 2)
            {
                Console.WriteLine($"Select the article number you are interested in:");
                isNumber = int.TryParse(Console.ReadLine(), out choice);
            }
            else
            {
                Console.WriteLine("Invalid choice. Please enter 1 or 2.");
            }
        }
        // if the number is out of range, prompt the user to enter a valid number
        else
        {
            Console.WriteLine("Invalid article number. Please try again.");
            Console.WriteLine($"Select the article number you are interested in:");
            isNumber = int.TryParse(Console.ReadLine(), out choice);
        }
    }
    // if the user input is not a number, prompt the user to enter a valid number
    else
    {
        Console.WriteLine("Invalid input. Please enter a number.");
        Console.WriteLine($"Select the article number you are interested in:");
        isNumber = int.TryParse(Console.ReadLine(), out choice);
    }
}
// Save the article information to a text file
File.WriteAllText("article.txt", $"Title: {article.Title}\nDescription: {article.Description}\nURL: {article.Url}");
//File.WriteAllText("articles.txt", $"Title: {articles[0].Title}\nDescription: {articles[0].Description}\nURL: {articles[0].Url}");

// Append the rest of the articles to the text file
File.Delete("articles.txt");
for (int j = 0; j<articles.Count; j++)
{
       File.AppendAllText("articles.txt", $"\n\nNews {j + 1}:\nTitle: {articles[j].Title}\nDescription: {articles[j].Description}\nURL: {articles[j].Url}");
}

Console.WriteLine(Path.GetFullPath("articles.txt"));



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace News_App
{
    internal class DisplayServices
    {

        public static void DisplaySingleArticle(Article article)
        {
            string output = $"Title: {article.Title}\nDescription: {article.Description}\nURL: {article.Url}";
            Console.WriteLine(output);
        }

        public static void DisplaySelectedArticle(List<Article> articles)
        {
            Console.WriteLine($"Select the new you are interested in: ");
            int choice;
            bool isNumber = int.TryParse(Console.ReadLine(), out choice);

            while (true)
            {
                if (isNumber)
                {
                    if (choice >= 1 && choice <= articles.Count)
                    {
                        Console.WriteLine($"News {choice}\nTitle: {articles[choice - 1].Title}\nDescription: {articles[choice - 1].Description}\nURL: {articles[choice - 1].Url}");
                        break;
                    }
                    else
                    {
                        Console.WriteLine($"Invalid number, choose between 1 and {articles.Count}");
                        isNumber = int.TryParse(Console.ReadLine(), out choice);
                    }
                }
                else
                {
                    Console.WriteLine($"Invalid number, choose between 1 and {articles.Count}");
                    isNumber = int.TryParse(Console.ReadLine(), out choice);
                }
            }
        }

        public static void DisplayMultipleArticles(List<Article> articles)
        {
            for(int i = 0; i<articles.Count; i++)
            {
                Console.WriteLine($"news {i + 1}\nTitle: {articles[i].Title}\nDescription: {articles[i].Description}\nURL: {articles[i].Url}");
            }
        }

        public static void DisplayArticleMetadata(List<Article> articles)
        {
            for(int i = 0; i<articles.Count; i++)
            {
                Console.WriteLine(
                    $"News {i + 1}:\n" +
                    $"Title: {articles[i].Title}\n" +
                    $"Description\n" +
                    $"Url: {articles[i].Url}\n" +
                    $"SourceName: {articles[i].SourceName}\n" +
                    $"PublishedAt: {articles[i].PublishedAt}\n" +
                    $"ImageUrl: {articles[i].ImageUrl}\n" +
                    $"ProviderArticleId: {articles[i].ProviderArticleId}\n" +
                    $"Language: {articles[i].Language}\n" +
                    $"Category: {articles[i].Category}");
            }
        }

        public static void DisplaySingleArticleMetadata(List<Article> articles)
        {
            Console.WriteLine($"Enter which news you want to see the metadata of: ");
            int choice;
            bool isNumber = int.TryParse(Console.ReadLine(), out choice);

            while (true)
            {
                if(isNumber)
                {
                    if (choice >0 && choice <= articles.Count)
                    {
                        Console.WriteLine(
                            $"News {choice}\n" +
                            $"Title: {articles[choice-1].Title}\n" +
                            $"Description: {articles[choice-1].Description}\n" +
                            $"Url: {articles[choice-1].Url}\n" +
                            $"SourceName: {articles[choice-1].SourceName}\n" +
                            $"PublishedAt: {articles[choice-1].PublishedAt}\n" +
                            $"ImageUrl: {articles[choice-1].ImageUrl}\n" +
                            $"ProviderArticleId: {articles[choice-1].ProviderArticleId}\n" +
                            $"Language: {articles[choice-1].Language}\n" +
                            $"Category: {articles[choice-1].Category}\n" +
                            $"Content: {articles[choice-1].Content}");
                        break;
                    }
                    else
                    {
                        Console.WriteLine($"Invalid number, choose 1");
                        isNumber = int.TryParse(Console.ReadLine(), out choice);
                    }
                }
                else
                {
                    Console.WriteLine($"Invalid number, choose 1");
                    isNumber = int.TryParse(Console.ReadLine(), out choice);
                }
            }
        }
    }
}

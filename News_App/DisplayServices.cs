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

        public static void DisplaySelectedArticles(List<Article> articles)
        {
            Console.WriteLine($"Select the new you are interested in: ");
            int choice;
            bool isNumber = int.TryParse(Console.ReadLine(), out choice);

            while (true)
            {
                if (isNumber)
                {
                    if (choice < 1 && choice > articles.Count)
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
    }
}

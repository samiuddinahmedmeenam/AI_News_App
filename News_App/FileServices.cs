using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace News_App
{
    internal class FileServices
    {
        public static void SaveArticleToFile(Article article)
        {
            string filepath = "articles.txt";
            string articleData = $"Title: {article.Title}\nDescription: {article.Description}\nURL: {article.Url}\n\n";
            File.AppendAllText(filepath, articleData);
        }

        public static void SaveMultipleArticlesToFile(List<Article> articles)
        {
            string filepath = "articles.txt";
            // File.Delete(filepath); // Clear the file before writing new data
            for (int j = 0; j < articles.Count; j++)
            {
                string articleData = $"News {j + 1}:\nTitle: {articles[j].Title}\nDescription: {articles[j].Description}\nURL: {articles[j].Url}\n\n";
                File.AppendAllText(filepath, articleData);
            }
        }
    }
}

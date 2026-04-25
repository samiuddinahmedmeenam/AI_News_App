using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace News_App
{
    internal class DatabaseService
    {
        private const string connectionString = "Data Source = news.db";

        public static void InitializeDatabase()
        {
            using var connection = new SqliteConnection(connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS NewsArticles (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    ProviderArticleId TEXT,
                    Title TEXT NOT NULL,
                    Description TEXT,
                    Url TEXT NOT NULL UNIQUE,
                    SourceName TEXT,
                    PublishedAt TEXT,
                    ImageUrl TEXT,
                    FetchedAt TEXT NOT NULL
                );
            ";
            command.ExecuteNonQuery();
        }

        public static void SaveArticle(Article article)
        {
            using var connection = new SqliteConnection(connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT OR IGNORE INTO NewsArticles 
                (ProviderArticleId, Title, Description, Url, SourceName, PublishedAt, ImageUrl, FetchedAt)
                VALUES ($ProviderArticleId, $Title, $Description, $Url, $SourceName, $PublishedAt, $ImageUrl, $FetchedAt);
            ";

            command.Parameters.AddWithValue("$ProviderArticleId", "");
            command.Parameters.AddWithValue("$Title", article.Title);
            command.Parameters.AddWithValue("$Description", article.Description);
            command.Parameters.AddWithValue("$Url", article.Url);
            command.Parameters.AddWithValue("$SourceName", article.SourceName);
            command.Parameters.AddWithValue("$PublishedAt", article.PublishedAt);
            command.Parameters.AddWithValue("$ImageUrl", article.ImageUrl);
            command.Parameters.AddWithValue("$FetchedAt", DateTime.UtcNow.ToString("O"));

            command.ExecuteNonQuery();
        }

        public static void SaveArticles(List<Article> articles)
        {
            foreach (var article in articles)
            {
                SaveArticle(article);
            }
        }

        public static void SaveSelectedArticle(List<Article> articles)
        {
                       Console.WriteLine("Enter the article you are interested to save: ");
            int choice;
            bool isNumber = int.TryParse(Console.ReadLine(), out choice);
            if (isNumber && choice > 0 && choice <= articles.Count)
            {
                Article selectedArticle = articles[choice - 1];
                SaveArticle(selectedArticle);
                Console.WriteLine("Selected article saved to database.");
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter a valid article number.");
            }
        }

        public static List<Article> GetAllArticles()
        {
            List<Article> articles = new List<Article>();

            using var connection = new SqliteConnection(connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText= @"
            SELECT Title, Description, Url, SourceName, PublishedAt, ImageUrl
            FROM NewsArticles;
            ";

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                Article article = new Article
                {
                    Title = reader.GetString(0),
                    Description = reader.IsDBNull(1) ? null : reader.GetString(1),
                    Url = reader.GetString(2),
                    SourceName = reader.IsDBNull(3) ? "" : reader.GetString(3),
                    PublishedAt = reader.IsDBNull(4) ? "" : reader.GetString(4),
                    ImageUrl = reader.IsDBNull(5) ? "" : reader.GetString(5)
                };

                articles.Add(article);
            }

            return articles;
        }
    }
}

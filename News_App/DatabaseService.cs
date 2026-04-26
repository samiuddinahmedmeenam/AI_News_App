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
                    Language TEXT,
                    Category TEXT,  
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
                (ProviderArticleId, Title, Description, Url, SourceName, PublishedAt, ImageUrl, Language, Category, FetchedAt)
                VALUES ($ProviderArticleId, $Title, $Description, $Url, $SourceName, $PublishedAt, $ImageUrl, $Language, $Category, $FetchedAt);
            ";

            command.Parameters.AddWithValue("$ProviderArticleId", article.ProviderArticleId);
            command.Parameters.AddWithValue("$Title", article.Title);
            command.Parameters.AddWithValue("$Description", article.Description);
            command.Parameters.AddWithValue("$Url", article.Url);
            command.Parameters.AddWithValue("$SourceName", article.SourceName);
            command.Parameters.AddWithValue("$PublishedAt", article.PublishedAt);
            command.Parameters.AddWithValue("$ImageUrl", article.ImageUrl);
            command.Parameters.AddWithValue("$Language", article.Language);
            command.Parameters.AddWithValue("$Category", article.Category);
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
            SELECT Title, Description, Url, SourceName, PublishedAt, ImageUrl, Language, Category
            FROM NewsArticles;
            ";

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                Article article = new Article
                {
                    ProviderArticleId = reader.IsDBNull(0) ? "" : reader.GetString(0),
                    Title = reader.IsDBNull(1) ? "" : reader.GetString(1),
                    Description = reader.IsDBNull(2) ? "" : reader.GetString(2),
                    Url = reader.IsDBNull(3) ? "" : reader.GetString(3),
                    SourceName = reader.IsDBNull(4) ? "" : reader.GetString(4),
                    PublishedAt = reader.IsDBNull(5) ? "" : reader.GetString(5),
                    ImageUrl = reader.IsDBNull(6) ? "" : reader.GetString(6),
                    Language = reader.IsDBNull(7) ? "" : reader.GetString(7),
                    Category = reader.IsDBNull(8) ? "" : reader.GetString(8)
                };

                articles.Add(article);
            }

            return articles;
        }

        public static bool ArticleExists()
        {
            using var connection = new SqliteConnection(connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = "SELECT COUNT(*) FROM NewsArticles;";

            long count = (long)command.ExecuteScalar();

            Console.WriteLine($"Total articles in database: {count}");
            return count > 0;
        }
    }
}

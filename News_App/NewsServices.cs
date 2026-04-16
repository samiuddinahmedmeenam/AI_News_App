using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;
using System.Text.Json;

// in order to move to API calls we need to learn3 concepts:
// 1. HttpClient: This class is used to send HTTP requests and receive HTTP responses
// 2. JSON Serialization: This is the process of converting .NET objects into JSON format and vice versa.
// 3. Asynchronous Programming: This is a programming paradigm that allows for non-blocking operations, which is essential for I/O-bound tasks like web requests.

// for now the next targets are to know:
// 1. making an HTTP request
// 2. getting raw JSON back as string
// 3. printing the JSON on the terminal


namespace News_App
{
    internal class NewsServices
    {
        public static async Task<List<Article>> GetTopNews()
        {
            // gets the raw JSON text
            string json = await CallAPI();
            // Deserialize teh response into a C# object
            NewsApiResponse? apiResponse = JsonSerializer.Deserialize<NewsApiResponse> (json);
            // Convert the API response into a list of Article objects
            List<Article> articles = new List<Article>();
            // Check if the API response is not null and contains results before processing
            if (apiResponse?.results != null)
            {
                for(int i = 0; i<apiResponse.results.Count; i++)
                {
                    NewsDataArticle item = apiResponse.results[i];
                    articles.Add(new Article
                    {
                        Title = item.title ?? "No title.",
                        Description = item.description ?? "No description.",
                        Url = item.link ?? "No URL.",
                        SourceName = item.source_name ?? "No source name.",
                        PublishedAt = item.pubDate ?? "No publication date.",
                        ImageUrl = item.image_url ?? "No image URL."
                    });
                }
            }
            return articles;
        }

        public static async Task<string> CallAPI()
        {
            using HttpClient Client = new HttpClient();
            string url = "https://newsdata.io/api/1/latest?apikey=pub_cbfa2a0ee9a44571a357aad0783424ad";
            string response = await Client.GetStringAsync(url);
            return response;
        }
    }

    // Collect Data from the JSON response by parsing it
    //get API response
    public class NewsApiResponse
    {
        public string? status { get; set; }
        public int totalResults { get; set; }
        public List<NewsDataArticle>? results { get; set; }
    }
    // get the article information from the JSON response and store it in a class
    public class NewsDataArticle
    {
        public string? article_id { get; set; } = "";
        public string? link { get; set; }
        public string? title { get; set; }
        public string? description { get; set; }
        public string? pubDate { get; set; }
        public string? image_url { get; set; }
        public string? source_name { get; set; }
        public string? source_url { get; set; }
    }
    // Create a class to represent the article information that we want to display
    

   
}



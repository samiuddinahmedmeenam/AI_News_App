using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace News_App
{
    internal class NewsServices
    {
        public static List<Article> GetTopNews()
        {
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

    // in order to move to API calls we need to learn3 concepts:
    // 1. HttpClient: This class is used to send HTTP requests and receive HTTP responses
    // 2. JSON Serialization: This is the process of converting .NET objects into JSON format and vice versa.
    // 3. Asynchronous Programming: This is a programming paradigm that allows for non-blocking operations, which is essential for I/O-bound tasks like web requests.

    // for now the next targets are to know:
    // 1. making an HTTP request
    // 2. getting raw JSON back as string
    // 3. printing the JSON on the terminal

}



using System.Net.Http;
using System.Text.Json;


namespace News_App
{
    public class NewsServices
    {
        public static async Task<List<Article>> GetTopNews()
        {
            string json = await CallAPI();

            NewsApiResponse? apiResponse = JsonSerializer.Deserialize<NewsApiResponse>(json);

            List<Article> articles = new List<Article>();

            if (apiResponse?.results != null)
            {
                for (int i = 0; i < apiResponse.results.Count && i < 5; i++)
                {
                    NewsDataArticle item = apiResponse.results[i];

                    articles.Add(new Article
                    {
                        Title = item.title ?? "No title.",
                        Description = item.description ?? "No description.",
                        Url = item.link ?? "No URL.",
                        SourceName = item.source_name ?? "No source name.",
                        PublishedAt = item.pubDate ?? "No publication date.",
                        ImageUrl = item.image_url ?? "No image URL.",
                        ProviderArticleId = item.article_id ?? "",
                        Language = item.language ?? "",
                        Category = item.category != null ? string.Join(",", item.category) : "",
                        Content = item.content ?? ""
                    });
                }
            }

            return articles;
        }

        public static async Task<string> CallAPI()
        {
            using HttpClient client = new HttpClient();
            string url = "https://newsdata.io/api/1/latest?apikey=pub_cbfa2a0ee9a44571a357aad0783424ad&language=en";
            string response = await client.GetStringAsync(url);
            return response;
        }
    }

    public class NewsApiResponse
    {
        public string? status { get; set; }
        public int totalResults { get; set; }
        public List<NewsDataArticle>? results { get; set; }
    }

    public class NewsDataArticle
    {
        public string? article_id { get; set; }
        public string? link { get; set; }
        public string? title { get; set; }
        public string? description { get; set; }
        public string? pubDate { get; set; }
        public string? image_url { get; set; }
        public string? source_name { get; set; }
        public string? source_url { get; set; }
        public string? language { get; set; }
        public List<string>? category { get; set; }
        public string? content { get; set; }
    }
}
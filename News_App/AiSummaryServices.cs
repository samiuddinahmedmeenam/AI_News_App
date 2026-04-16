using System.Net.Http;
using System.Text;
using System.Text.Json;


namespace News_App
{
    internal class AiSummaryServices
    {
        public static async Task<string> GetSummary(Article selectedArticle)
        {
            string? apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                return "OpenAI API key not found. Set OPENAI_API_KEY first.";
            }

            using HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

            string prompt =
                $"Summarize this news article in 3 short lines.\n" +
                $"Title: {selectedArticle.Title}\n" +
                $"Description: {selectedArticle.Description}";

            var bodyObject = new
            {
                model = "gpt-4o",
                input = prompt,
                max_output_tokens = 120
            };

            string jsonBody = JsonSerializer.Serialize(bodyObject);

            using StringContent content = new StringContent(
                jsonBody,
                Encoding.UTF8,
                "application/json"
            );

            HttpResponseMessage response =
                await client.PostAsync("https://api.openai.com/v1/responses", content);

            string responseJson = await response.Content.ReadAsStringAsync();
            Console.WriteLine("RAW OPENAI RESPONSE:");
            Console.WriteLine(responseJson);

            if (!response.IsSuccessStatusCode)
            {
                return $"OpenAI API error: {response.StatusCode}\n{responseJson}";
            }

            using JsonDocument doc = JsonDocument.Parse(responseJson);

            if (doc.RootElement.TryGetProperty("output", out JsonElement outputArray))
            {
                foreach (JsonElement item in outputArray.EnumerateArray())
                {
                    if (item.TryGetProperty("content", out JsonElement contentArray))
                    {
                        foreach (JsonElement contentItem in contentArray.EnumerateArray())
                        {
                            if (contentItem.TryGetProperty("text", out JsonElement textElement))
                            {
                                return textElement.GetString() ?? "No summary returned.";
                            }
                        }
                    }
                }
            }

            return "No summary text found in response.";
        }
    }
}
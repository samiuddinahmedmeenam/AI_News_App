using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace News_App
{
    internal class EmbeddingService
    {
        public static async Task<List<float>> GetEmbedding(string text)
        {
            string? apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new Exception("OpenAI API key not found.");
            }

            using HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

            var bodyObject = new
            {
                model = "text-embedding-3-small",
                input = text
            };

            string jsonBody = JsonSerializer.Serialize(bodyObject);

            using StringContent content = new StringContent(
                jsonBody,
                Encoding.UTF8,
                "application/json"
            );

            HttpResponseMessage response =
                await client.PostAsync("https://api.openai.com/v1/embeddings", content);

            string responseJson = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Embedding API error: {response.StatusCode}\n{responseJson}");
            }

            using JsonDocument doc = JsonDocument.Parse(responseJson);

            JsonElement embeddingArray =
                doc.RootElement
                   .GetProperty("data")[0]
                   .GetProperty("embedding");

            List<float> embedding = new List<float>();

            foreach (JsonElement value in embeddingArray.EnumerateArray())
            {
                embedding.Add(value.GetSingle());
            }

            return embedding;
        }



    }
}
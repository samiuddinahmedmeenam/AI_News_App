using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace News_App
{
    internal class RetrievalService
    {
        public static List<ArticleChunk> RetrieveRelevantChunks(string question, List<ArticleChunk> chunks, int topK = 3)
        {
            List<string> keywords = ExtractKeywords(question);

            var scoredChunks = new List<(ArticleChunk chunk, int score)>();

            foreach(var chunk in chunks)
            {
                int score = 0;
                string chunkTextLower = chunk.ChunkText.ToLower();

                foreach(var keyword in keywords)
                {
                    if (chunkTextLower.Contains(keyword)){
                        score++;
                    }
                }

                if(score > 0)
                {
                    scoredChunks.Add((chunk, score));
                }
            }

            return scoredChunks.OrderByDescending(x => x.score)
                               .Take(topK)
                               .Select(x => x.chunk)
                               .ToList();
        }

        public static List<(ArticleChunk chunk, int score)> RetrieveRelevantChunksWithScores(string question, List<ArticleChunk> chunks, int topK = 3)
        {
            List<string> keywords = ExtractKeywords(question);

            var scoredChunks = new List<(ArticleChunk chunk, int score)>();

            foreach (var chunk in chunks)
            {
                int score = 0;

                HashSet<string> chunkWords = Regex.Split(chunk.ChunkText.ToLower(), @"\W+")
                    .Where(word => !string.IsNullOrWhiteSpace(word))
                    .ToHashSet();

                foreach (var keyword in keywords)
                {
                    if (chunkWords.Contains(keyword))
                    {
                        score++;
                    }
                }

                if (score > 0)
                {
                    scoredChunks.Add((chunk, score));
                }
            }

            return scoredChunks
                .OrderByDescending(x => x.score)
                .ThenBy(x => x.chunk.ChunkIndex)
                .Take(topK)
                .ToList();
        }



        private static List<string> ExtractKeywords(string question)
        {
            return Regex.Split(question.ToLower(), @"\W+")
                        .Where(word => !string.IsNullOrWhiteSpace(word) && word.Length > 2)
                        .Distinct()
                        .ToList();
        }

        public static string BuildContextFromChunks(List<ArticleChunk> chunks)
        {
            if (chunks == null || chunks.Count == 0)
            {
                return "";
            }

            return string.Join("\n\n---\n\n", chunks.Select(c => c.ChunkText));
        }

        public static double CosineSimilarity(List<float> a, List<float> b)
        {
            double dot = 0;
            double normA = 0;
            double normB = 0;

            for (int i = 0; i < a.Count; i++)
            {
                dot += a[i] * b[i];
                normA += a[i] * a[i];
                normB += b[i] * b[i];
            }

            return dot / (Math.Sqrt(normA) * Math.Sqrt(normB));
        }

        public static List<ArticleChunk> RetrieveRelevantChunksSemantic(
                                                                        List<float> questionEmbedding,
                                                                        List<ArticleChunk> chunks,
                                                                        Dictionary<int, List<float>> chunkEmbeddings,
                                                                        int topK = 5)
        {
            var scored = new List<(ArticleChunk chunk, double score)>();

            foreach (var chunk in chunks)
            {
                if (chunkEmbeddings.TryGetValue(chunk.Id, out var embedding))
                {
                    double score = CosineSimilarity(questionEmbedding, embedding);
                    scored.Add((chunk, score));
                }
            }

            return scored
                .OrderByDescending(x => x.score)
                .Take(topK)
                .Select(x => x.chunk)
                .ToList();
        }
    }
}

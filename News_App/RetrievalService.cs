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

        private static List<string> ExtractKeywords(string question)
        {
            return Regex.Split(question.ToLower(), @"\W+")
                        .Where(word => !string.IsNullOrWhiteSpace(word) && word.Length > 2)
                        .Distinct()
                        .ToList();
        }
    }
}

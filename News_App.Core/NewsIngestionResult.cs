namespace News_App
{
    public class NewsIngestionResult
    {
        public string Message { get; set; } = "";

        public int FetchedArticles { get; set; }
        public int SavedArticles { get; set; }
        public int SkippedArticles { get; set; }
        public int TotalArticles { get; set; }

        public int TotalChunks { get; set; }

        public int NewEmbeddings { get; set; }
        public int SkippedEmbeddings { get; set; }

        public DateTime StartedAt { get; set; }
        public DateTime FinishedAt { get; set; }
        public double DurationSeconds { get; set; }
    }
}
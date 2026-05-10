namespace News_App
{
    public class NewsIngestionResult
    {
        public string Message { get; set; } = "";
        public int FetchedArticles { get; set; }
        public int TotalChunks { get; set; }
        public int NewEmbeddings { get; set; }
        public int SkippedEmbeddings { get; set; }
    }
}
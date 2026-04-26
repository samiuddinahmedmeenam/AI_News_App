using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace News_App
{
    internal class ArticleChunk
    {
        public int Id {  get; set; }
        public string ArticleUrl { get; set; } = "";
        public string ProviderArticleId { get; set; } = "";
        public int ChunkIndex { get; set; }
        public string ChunkText { get; set; } = "";
        public string CreatedAt { get; set; } = "";
    }
}

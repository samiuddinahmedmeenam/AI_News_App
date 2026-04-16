using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace News_App
{
    internal class AiSummaryServices
    {
        public static async Task<string> GetSummary(Article selectedArticle)
        {
            string summary = "summary of the selected article: ";
            await Task.CompletedTask;
            return summary;
        }
    }
}

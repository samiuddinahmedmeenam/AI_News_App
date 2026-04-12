using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }


}

namespace Shopily.Domain.Entity
{
    public class NewsArticle
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public DateTime? DatePublished { get; set; }
        public string Content { get; set; }
        public DateTime DateScraped { get; set; }
        public string SourceWebsite { get; set; }
    }
}

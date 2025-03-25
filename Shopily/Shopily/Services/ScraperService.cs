using HtmlAgilityPack;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Shopily.Domain.Entity;

public class ScraperService
{
    private readonly HttpClient _httpClient = new HttpClient();

    public async Task<List<NewsArticle>> ScrapeWebsiteAsync(string url)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/132.0.6834.196 Safari/537.36");
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var htmlContent = await response.Content.ReadAsStringAsync();

            var document = new HtmlDocument();
            document.LoadHtml(htmlContent);

            // Extract articles from schema.org data
            var schemaArticles = ExtractSchemaArticles(document);

            // Extract articles from HTML structure
            var htmlArticles = ExtractHtmlArticles(document, url);

            // Combine and deduplicate
            return CombineAndDeduplicate(schemaArticles, htmlArticles);
        }
        catch
        {
            return new List<NewsArticle>();
        }
    }

    private List<NewsArticle> ExtractSchemaArticles(HtmlDocument document)
    {
        var articles = new List<NewsArticle>();
        var scriptNodes = document.DocumentNode.SelectNodes("//script[@type='application/ld+json']");

        if (scriptNodes != null)
        {
            foreach (var script in scriptNodes)
            {
                try
                {
                    var jsonData = JObject.Parse(script.InnerText);

                    var type = jsonData.GetValue("@type", StringComparison.OrdinalIgnoreCase)?.ToString();
                    if (string.Equals(type, "NewsArticle", StringComparison.OrdinalIgnoreCase))
                    {
                        var article = new NewsArticle
                        {
                            Title = jsonData.GetValue("headline")?.ToString(),
                            Author = GetAuthor(jsonData["author"]?.ToString()),
                            DatePublished = DateTime.TryParse(jsonData.GetValue("datePublished")?.ToString(), out DateTime parsedDate) ? parsedDate : (DateTime?)null,
                            Content = jsonData.GetValue("articleBody")?.ToString(),
                            Url = jsonData.GetValue("url")?.ToString()
                        };
                        articles.Add(article);
                    }
                }
                catch (Exception ex)
                {
                    // Improved error handling
                    Console.WriteLine($"Error processing JSON-LD data: {ex.Message}");
                }
            }
        }

        return articles;
    }

    private string GetAuthor(JToken authorData)
    {
        if (authorData is JObject authorObject)
            return authorObject["name"]?.ToString();
        else if (authorData is JArray authorArray && authorArray.Count > 0)
            return authorArray[0]["name"]?.ToString();
        else
            return null;
    }

    private List<NewsArticle> ExtractHtmlArticles(HtmlDocument document, string baseUrl)
    {
        var articles = new List<NewsArticle>();
        var articleNodes = new[]
        {
            document.DocumentNode.SelectNodes("//article"), // Direct article tags
            document.DocumentNode.SelectNodes("//div[contains(@class, 'article') or contains(@class, 'post') or contains(@class, 'news-item') or contains(@class, 'content')]"),
            document.DocumentNode.SelectNodes("//section[contains(@class, 'article') or contains(@class, 'news-item') or contains(@class, 'content')]")
        };

        foreach (var nodes in articleNodes)
        {
            if (nodes != null)
            {
                foreach (var node in nodes)
                {
                    var article = new NewsArticle
                    {
                        Url = ExtractUrl(node, baseUrl),
                        Title = ExtractTitle(node),
                        Author = ExtractAuthor(node),
                        DatePublished = ExtractDate(node),
                        Content = ExtractContent(node)
                    };

                    if (!string.IsNullOrEmpty(article.Title))
                        articles.Add(article);
                }
            }
        }

        return articles;
    }

    private string ExtractUrl(HtmlNode node, string baseUrl)
    {
        var anchor = node.SelectSingleNode(".//a");
        return anchor?.Attributes["href"] != null ? new Uri(new Uri(baseUrl), anchor.Attributes["href"].Value).AbsoluteUri : null;
    }

    private string ExtractTitle(HtmlNode node)
    {
        var title = node.SelectSingleNode(".//h1")?.InnerText
                    ?? node.SelectSingleNode(".//h2")?.InnerText
                    ?? node.SelectSingleNode(".//h3")?.InnerText;
        return title?.Trim();
    }

    private string ExtractAuthor(HtmlNode node)
    {
        var author = node.SelectSingleNode(".//div[contains(@class, 'author') or contains(@class, 'byline')]")
                  ?? node.SelectSingleNode(".//span[contains(@class, 'author') or contains(@class, 'byline')]");
        return author?.InnerText?.Trim();
    }

    private DateTime? ExtractDate(HtmlNode node)
    {
        var date = node.SelectSingleNode(".//time")?.Attributes["datetime"]?.Value;
        return DateTime.TryParse(date, out DateTime result) ? result : (DateTime?)null;
    }

    private string ExtractContent(HtmlNode node)
    {
        var paragraphs = node.SelectNodes(".//p");
        return paragraphs != null ? string.Join("\n", paragraphs.Select(p => p.InnerText.Trim())) : null;
    }

    private List<NewsArticle> CombineAndDeduplicate(List<NewsArticle> schemaArticles, List<NewsArticle> htmlArticles)
    {
        var combined = new List<NewsArticle>(schemaArticles);
        combined.AddRange(htmlArticles);
        combined = combined.GroupBy(a => a.Title)
                           .Where(g => !string.IsNullOrEmpty(g.Key))
                           .Select(g => g.First())
                           .ToList();
        return combined;
    }
}
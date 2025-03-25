using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopily.Data;

public class NewsController : Controller
{
    private readonly Context _context;
    private readonly ScraperService _scraperService;

    public NewsController(Context context, ScraperService scraperService)
    {
        _context = context;
        _scraperService = scraperService;
    }
    public IActionResult Scraper()
    {
        return View();
    }
    public async Task<IActionResult> ListArticles()
    {
        var articles = await _context.NewsArticles.ToListAsync();
        return View(articles);
    }

    [HttpPost]
    public async Task<IActionResult> Scrape(string url)
    {
        if (string.IsNullOrEmpty(url))
        {
            ViewBag.Message = "Please enter a valid URL.";
            return View("Scraper");
        }

        try
        {
            var articles = await _scraperService.ScrapeWebsiteAsync(url);

            foreach (var article in articles)
            {
                article.SourceWebsite = url;
                _context.NewsArticles.Add(article);
            }

            await _context.SaveChangesAsync();
            ViewBag.Message = $"{articles.Count} articles successfully scraped and saved!";
        }
        catch (Exception ex)
        {
            ViewBag.Message = $"Error during scraping: {ex.Message}";
        }

        return View("Scraper");
    }
}
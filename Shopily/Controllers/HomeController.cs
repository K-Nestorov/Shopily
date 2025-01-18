using Microsoft.AspNetCore.Mvc;
using Shopily.Cookies;
using Shopily.Entity;
using Shopily.Repositories;
using Shopily.ViewModel.Products;
using System.Diagnostics;
using System.Text.Json;

namespace Shopily.Controllers
{

    public class HomeController : Controller
    {
        private readonly Context context;

      
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, Context _context)
        {
            _logger = logger; context = _context;
        }
        public IActionResult Index()
        {
            var model = new IndexVM
            {
                Items = context.Products.ToList() 
            };
<<<<<<< HEAD

            
           
=======
            var recentlyViewedJson = Request.Cookies["RecentlyViewed"];
            List<RecentlyViewedCookie> recentlyViewedProducts = new List<RecentlyViewedCookie>();
            if (!string.IsNullOrEmpty(recentlyViewedJson))
            {
                try
                {
                    recentlyViewedProducts = JsonSerializer.Deserialize<List<RecentlyViewedCookie>>(recentlyViewedJson);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error deserializing cookie: {ex.Message}");
                    recentlyViewedProducts = new List<RecentlyViewedCookie>();
                }
            }
            ViewData["RecentlyViewed"] = recentlyViewedProducts;
           // var recentlyViewedProducts = ViewData["RecentlyViewed"] as List<Shopily.ViewModel.Products.RecentlyViewedCookie>;

>>>>>>> e752cef18d7408f29f6e3814efda787e4d92bb84

            return View(model);
           
        }
        [Route("About")]
        public IActionResult About()
        {
            return View();
        }
        [Route("Contact")]
        public IActionResult Contact()
        {
            return View();
        }
        [Route("Thankyou")]
        public IActionResult Thankyou()
        {
            return View();
        }
        

    }
}

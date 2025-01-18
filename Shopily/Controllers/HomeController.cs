using Microsoft.AspNetCore.Mvc;
using Shopily.Cookies;
using Shopily.Entity;
using Shopily.Repositories;
using Shopily.ViewModel.Products;
using System.Diagnostics;

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

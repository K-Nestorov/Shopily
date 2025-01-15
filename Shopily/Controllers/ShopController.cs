using Microsoft.AspNetCore.Mvc;

namespace Shopily.Controllers
{
    public class ShopController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        [Route("Cart")]
        public IActionResult Cart()
        {
            return View();
        }
        [Route("Checkout")]
        public IActionResult Checkout()
        {
            return View();
        }

        [Route("Shop")]
        public IActionResult Shop()
        {

            return View();
        }
        [Route("Shopsingle")]
        public IActionResult Shopsingle()
        {
            return View();
        }

    }
}

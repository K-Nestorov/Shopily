using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopily.Cookies;
using Shopily.Entity;
using Shopily.Repositories;
using Shopily.ViewModel;
using Shopily.ViewModel.Admin;
using Shopily.ViewModel.Cart;
using Shopily.ViewModel.Pages;
using Shopily.ViewModel.Products;
using Shopily.ViewModel.Pages;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

namespace Shopily.Controllers
{
    public class ShopController : Controller
    {
        private readonly Context _context;

        public ShopController(Context context)
        {
            _context = context;
        }

      
        private IEnumerable<Product> GetProductsForPage(int page, int itemsPerPage)
        {
            return _context.Products
                .Skip((page - 1) * itemsPerPage)
                .Take(itemsPerPage)
                .ToList();
        }

        
        private int GetTotalItemsCount()
        {
            return _context.Products.Count();
        }


        [Route("Cart")]
        public IActionResult Cart(int page = 1)
        {
            // Retrieve the logged user
            if (User.Identity == null || !User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "User");
            }

            // Retrieve the user ID from claims
            var userIdClaim = User.FindFirst(ClaimTypes.Sid)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
            {
                return RedirectToAction("Login", "User");
            }

            // Fetch cart items for the logged user
            var cartItems = _context.Carts
                .Where(c => c.UserId == userId) // Ensure UserId is of type Guid in your database
                .Include(c => c.Product)
                .ToList();


            int itemsPerPage = 12;
            var products = GetProductsForPage(page, itemsPerPage);
            var totalProducts = GetTotalItemsCount();
            var pagesCount = (int)Math.Ceiling((double)totalProducts / itemsPerPage);



            var model = new ViewModel.Cart.IndexVM // tuk trqbva da e cart.indexvm 
            {
              Items  = cartItems.Select(c => new CartItemVM
                {
                    ProductId = c.ProductId,
                    ProductName = c.Product.ProductName,
                    Quantity = 1,
                    Price = c.Product.Price
                }).ToList(),

               
            };
            return View(model);
        }

        public IActionResult AddInCart(int clickedId)
        {
            User loggedUser = Request.Cookies.GetObject<User>("UserInfo");
            if (loggedUser == null)
            {
                return RedirectToAction("Login");
            }

            var product = _context.Products.FirstOrDefault(p => p.Id == clickedId);

            if (product == null)
            {
                ModelState.AddModelError("", "Product not found.");
                return RedirectToAction("Index", "Home");
            }

           
            var existingCartItem = _context.Carts
                .FirstOrDefault(c => c.ProductId == clickedId && c.UserId == loggedUser.Id);

            if (existingCartItem == null)
            {
                
                var newCartItem = new Cart
                {
                    UserId = loggedUser.Id,
                    ProductId = product.Id
                };

                _context.Carts.Add(newCartItem);
                _context.SaveChanges();
            }

            return RedirectToAction("Index", "Home");
        }
        //Task ili void
       public IActionResult Delete(int productId, int UserId)
        {
          
            User loggedUser = Request.Cookies.GetObject<User>("UserInfo");


            if (loggedUser == null)
            {
                return RedirectToAction("Login", "Home");
            }

            var cartItem = _context.Carts
                .FirstOrDefault(c => c.UserId == loggedUser.Id && c.ProductId == productId);
            if (cartItem == null)
            {
                return RedirectToAction("Index");
            }

            _context.Carts.Remove(cartItem);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [Route("Checkout")]
        public IActionResult Checkout()
        {
            return View();
        }

        [Route("Shop")]
        public IActionResult Shop(string productName, string category, string productType, int page = 1)
        {
            int itemsPerPage = 12;

            var allProducts = GetAllProducts();

            if (!string.IsNullOrEmpty(productName))
            {
                allProducts = allProducts.Where(p => p.ProductName.Contains(productName, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (!string.IsNullOrEmpty(category))
            {
                allProducts = allProducts.Where(p => p.Gender == category).ToList();
            }

            if (!string.IsNullOrEmpty(productType))
            {
                allProducts = allProducts.Where(p => p.Type == productType).ToList();
            }

            var maleCount = allProducts.Count(p => p.Gender == "Male");
            var femaleCount = allProducts.Count(p => p.Gender == "Female");
            var kidsCount = allProducts.Count(p => p.Gender == "Kids");


            var totalProducts = allProducts.Count();
            var pagesCount = (int)Math.Ceiling((double)totalProducts / itemsPerPage);
            var products = allProducts.Skip((page - 1) * itemsPerPage).Take(itemsPerPage).ToList();

            var model = new ViewModel.Products.IndexVM
            {
                Items = products,
                Pager = new PagerVM
                {
                    ItemsPerPage = itemsPerPage,
                    PagesCount = pagesCount,
                    CurrentPage = page
                },
                ProductName = productName,
                Category = category,
                ProductType = productType,
                MaleCount = maleCount,
                FemaleCount = femaleCount,
                KidsCount = kidsCount
            };

            return View(model);
        }

        private List<Product> GetAllProducts()
        {
           
            return _context.Products.ToList(); 
        }
        public IActionResult Shopsingle(int ProductId)
        {
            // Retrieve the product by ID
            Product? item = _context.Products.FirstOrDefault(u => u.Id == ProductId);
            if (item == null)
            {
                return RedirectToAction("Index", "Admin");
            }

            // Create the view model for the product
            SingleProductVM model = new SingleProductVM
            {
                Id = item.Id,
                ProductName = item.ProductName,
                Description = item.Description,
                Price = item.Price,
                Gender = item.Gender,
                Image = item.ImagePath,
            };

            // Retrieve the existing "LastViewedProduct" cookie
            var lastViewedProductJson = Request.Cookies["LastViewedProduct"];
            List<RecentlyViewedProduct> lastViewedProducts = new List<RecentlyViewedProduct>();

            if (!string.IsNullOrEmpty(lastViewedProductJson))
            {
                try
                {
                    // Deserialize the JSON into a list of products
                    lastViewedProducts = System.Text.Json.JsonSerializer.Deserialize<List<RecentlyViewedProduct>>(lastViewedProductJson) ?? new List<RecentlyViewedProduct>();
                }
                catch (System.Text.Json.JsonException)
                {
                    // If deserialization fails, log the error (optional) and reset the list
                    lastViewedProducts = new List<RecentlyViewedProduct>();
                }
            }

            // Remove the product if it already exists to avoid duplicates
            lastViewedProducts = lastViewedProducts.Where(p => p.Id != model.Id).ToList();

            // Add the current product at the top of the list
            lastViewedProducts.Insert(0, new RecentlyViewedProduct
            {
                Id = model.Id,
                ProductName = model.ProductName,
                Price = model.Price,
                Image = model.Image // Include image if needed
            });

            // Limit to the last 5 products
            lastViewedProducts = lastViewedProducts.Take(5).ToList();

            // Serialize the updated list back into a JSON string
            string updatedProductCookieJson = System.Text.Json.JsonSerializer.Serialize(lastViewedProducts);

            // Update the cookie with the new list of products
            Response.Cookies.Append("LastViewedProduct", updatedProductCookieJson, new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddHours(1), // Set expiration time
                HttpOnly = true, // Secure the cookie
                SameSite = SameSiteMode.Lax // Adjust as needed
            });

            // Return the view with the current product
            return View(model);
        }


        public IActionResult RecentlyViewed()
        {
            // Retrieve the "LastViewedProduct" cookie
            var lastViewedProductJson = Request.Cookies["LastViewedProduct"];
            List<RecentlyViewedProduct> lastViewedProducts = new List<RecentlyViewedProduct>();

            if (!string.IsNullOrEmpty(lastViewedProductJson))
            {
                try
                {
                    // Deserialize the JSON string back into a list of products
                    lastViewedProducts = System.Text.Json.JsonSerializer.Deserialize<List<RecentlyViewedProduct>>(lastViewedProductJson) ?? new List<RecentlyViewedProduct>();
                }
                catch (System.Text.Json.JsonException)
                {
                    // Log the error (optional) and reset the list
                    lastViewedProducts = new List<RecentlyViewedProduct>();
                }
            }

            // Get all products (if needed for the view)
            var products = _context.Products.ToList();

            // Create the view model and pass both products and recently viewed products
            var model = new ViewModel.Products.IndexVM
            {
                Items = products,
                RecentlyViewedProducts = lastViewedProducts
            };

            // Pass the view model to the view
            return View(model);
        }








    }
}

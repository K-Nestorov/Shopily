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
using System.Text.Json;

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

        [Route("Liked")]
        public IActionResult Like(int page=1)
        {
            var recentlyViewedJson = Request.Cookies["AddInLike"];
            List<CartItemVM> recentlyViewedProducts = new List<CartItemVM>();
            if (!string.IsNullOrEmpty(recentlyViewedJson))
            {
                try
                {
                    recentlyViewedProducts = JsonSerializer.Deserialize<List<CartItemVM>>(recentlyViewedJson);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error deserializing cookie: {ex.Message}");
                    recentlyViewedProducts = new List<CartItemVM>();
                }
            }

            int itemsPerPage = 12;
            var products = GetProductsForPage(page, itemsPerPage);
            var totalProducts = GetTotalItemsCount();
            var pagesCount = (int)Math.Ceiling((double)totalProducts / itemsPerPage);
            ViewData["AddInLike"] = recentlyViewedProducts;
            return View();
        }


        [Route("Cart")]
        public IActionResult Cart(int page = 1)
        {
            // Retrieve the logged user


            if (!User.Identity.IsAuthenticated)
            {
                var recentlyViewedJson = Request.Cookies["AddInCart"];
                List<CartItemVM> recentlyViewedProducts = new List<CartItemVM>();
                if (!string.IsNullOrEmpty(recentlyViewedJson))
                {
                    try
                    {
                        recentlyViewedProducts = JsonSerializer.Deserialize<List<CartItemVM>>(recentlyViewedJson);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error deserializing cookie: {ex.Message}");
                        recentlyViewedProducts = new List<CartItemVM>();
                    }
                }
                ViewData["AddInCart"] = recentlyViewedProducts;
            }
            else
            {


                var loggedUserId = Guid.Parse(User.FindFirst(ClaimTypes.Sid)?.Value);

                
                var cartItems = _context.Carts
                    .Where(c => c.UserId == loggedUserId) 
                    .Include(c => c.Product)
                    .ToList();


        
                int itemsPerPage = 12;
                var products = GetProductsForPage(page, itemsPerPage);
                var totalProducts = GetTotalItemsCount();
                var pagesCount = (int)Math.Ceiling((double)totalProducts / itemsPerPage);



                var model = new ViewModel.Cart.IndexVM 
                {
                    Items = cartItems.Select(c => new CartItemVM
                    {
                        ProductId = c.ProductId,
                        ProductName = c.Product.ProductName,
                        Quantity = 1,
                        Price = c.Product.Price,
                        ImagePath=c.Product.ImagePath,
                        
                    }).ToList(),


                };
                return View(model);
            }
            return View();
           
        }

        public IActionResult AddInCart(int clickedId,int likedId)
        {
            if (likedId != 0)
            {
                Product? item = _context.Products.FirstOrDefault(u => u.Id == likedId);
                var AddInLikeJson = Request.Cookies["AddInLike"];
                List<CartItemVM> AddInLike = new List<CartItemVM>();
                if (!string.IsNullOrEmpty(AddInLikeJson))
                {
                    AddInLike = JsonSerializer.Deserialize<List<CartItemVM>>(AddInLikeJson);
                }
                AddInLike.Add(new CartItemVM
                {
                    ProductId = item.Id,
                    ProductName = item.ProductName,
                    ImagePath = item.ImagePath,
                    Price = item.Price,
                    //QUANTITY

                });
                var jsonData = JsonSerializer.Serialize(AddInLike);
                Response.Cookies.Append("AddInLike", jsonData, new CookieOptions
                {
                    Expires = DateTimeOffset.Now.AddDays(1)
                });
                var AddIn = HttpContext.Request.Cookies["AddInLike"];
            }
            else
            {


                if (!User.Identity.IsAuthenticated)
                {
                    Product? item = _context.Products.FirstOrDefault(u => u.Id == clickedId);
                    var AddInCartJson = Request.Cookies["AddInCart"];
                    List<CartItemVM> AddInCart = new List<CartItemVM>();
                    if (!string.IsNullOrEmpty(AddInCartJson))
                    {
                        AddInCart = JsonSerializer.Deserialize<List<CartItemVM>>(AddInCartJson);
                    }
                    AddInCart.Add(new CartItemVM
                    {
                        ProductId = item.Id,
                        ProductName = item.ProductName,
                        ImagePath = item.ImagePath,
                        Price = item.Price,
                        //QUANTITY

                    });
                    var jsonData = JsonSerializer.Serialize(AddInCart);
                    Response.Cookies.Append("AddInCart", jsonData, new CookieOptions
                    {
                        Expires = DateTimeOffset.Now.AddDays(1)
                    });
                    var AddIn = HttpContext.Request.Cookies["AddInCart"];
                }
                else
                {

                    var loggedUserId = Guid.Parse(User.FindFirst(ClaimTypes.Sid)?.Value);

                    var product = _context.Products.FirstOrDefault(p => p.Id == clickedId);
                    if (product == null)
                    {
                        ModelState.AddModelError("", "Product not found.");
                        return RedirectToAction("Index", "Home");
                    }

                    var existingCartItem = _context.Carts
                        .FirstOrDefault(c => c.ProductId == clickedId && c.UserId == loggedUserId);

                    if (existingCartItem == null)
                    {

                        var newCartItem = new Cart
                        {
                            UserId = loggedUserId,
                            ProductId = product.Id

                        };

                        _context.Carts.Add(newCartItem);
                        _context.SaveChanges();
                    }
                }
            }

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Delete(int productId, int UserId)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Home");
            }

            var loggedUserId = Guid.Parse(User.FindFirst(ClaimTypes.Sid)?.Value);

            var cartItem = _context.Carts
                .FirstOrDefault(c => c.UserId == loggedUserId && c.ProductId == productId);
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
            // Retrieve the product
            Product? item = _context.Products.FirstOrDefault(u => u.Id == ProductId);
            if (item == null)
            {
                return RedirectToAction("Index", "Admin");
            }

            // Create the SingleProductVM for the view
            SingleProductVM model = new SingleProductVM
            {
                Id = item.Id,
                ProductName = item.ProductName,
                Description = item.Description,
                Price = item.Price,
                Gender = item.Gender,
                Image = item.ImagePath,
            };

            // Get the cookie
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

            // Add the product to the recently viewed list if not already present
            if (recentlyViewedProducts.All(p => p.ProductId != item.Id))
            {
                recentlyViewedProducts.Add(new RecentlyViewedCookie
                {
                    ProductId = item.Id,
                    ProductName = item.ProductName,
                    Description = item.Description,
                    Price = item.Price,
                    ImagePath = item.ImagePath,
                });
            }

            // Serialize and save the updated list back to the cookie
            var jsonData = JsonSerializer.Serialize(recentlyViewedProducts);
            Response.Cookies.Append("RecentlyViewed", jsonData, new CookieOptions
            {
                Expires = DateTimeOffset.Now.AddMinutes(30)
            });

            // Debugging: Verify the cookie data
            Console.WriteLine($"Serialized Recently Viewed Products: {jsonData}");

            // Pass the data to the view
            var recentlyViewed = HttpContext.Request.Cookies["RecentlyViewed"];
            ViewBag.RecentlyViewed = recentlyViewed;

            return View(model);
        }






    }
}

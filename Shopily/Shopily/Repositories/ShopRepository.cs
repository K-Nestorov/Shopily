using Microsoft.EntityFrameworkCore;
using Shopily.Data;
using Shopily.Domain.Entity;
using Shopily.Domain.ViewModel.Cart;
using Shopily.Domain.ViewModel.Products;
using Shopily.Domain.ViewModel.Pages;
using System.Text.Json;

namespace Shopily.Repositories
{
    public class ShopRepository
    {
        private readonly Context _context;

        public ShopRepository(Context context)
        {
            _context = context;
        }



        public Domain.ViewModel.Products.IndexVM CreateShopVM(string productName, string category, string[] productTypes, int page = 1)
        {
            int itemsPerPage = 12;

            var allProducts = _context.Products.AsQueryable();

            if (!string.IsNullOrEmpty(category))
            {
                var categories = category.Split(',');  
                allProducts = allProducts.Where(p => categories.Contains(p.Gender));
            }

            if (!string.IsNullOrEmpty(productName))
            {
                allProducts = allProducts.Where(p => p.ProductName.ToLower().Contains(productName.ToLower()));
            }

            if (productTypes != null && productTypes.Any())
            {
                allProducts = allProducts.Where(p => productTypes
                    .Select(type => type.ToLower())
                    .Contains(p.Type.ToLower()));
            }

            var allProductsList = allProducts.ToList();

            var maleCount = allProductsList.Count(p => p.Gender == "Male");
            var femaleCount = allProductsList.Count(p => p.Gender == "Female");
            var kidsCount = allProductsList.Count(p => p.Gender == "Kids");

            var totalProducts = allProductsList.Count();
            var pagesCount = (int)Math.Ceiling((double)totalProducts / itemsPerPage);

            var products = allProductsList.Skip((page - 1) * itemsPerPage).Take(itemsPerPage).ToList();

            var model = new Domain.ViewModel.Products.IndexVM
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
                ProductType = productTypes != null ? string.Join(",", productTypes) : "", 

                MaleCount = maleCount,
                FemaleCount = femaleCount,
                KidsCount = kidsCount
            };

            return model;
        }






        public List<Product> GetProducts(int page, int itemsPerPage)
        {
            return _context.Products
                .Skip((page - 1) * itemsPerPage)
                .Take(itemsPerPage)
                .ToList();
        }

        public int GetTotalProductsCount()
        {
            return _context.Products.Count();
        }

        public List<CartItemVM> GetCartItems(Guid userId)
        {
            return _context.Carts
                .Where(c => c.UserId == userId)
                .Include(c => c.Product)
                .Select(c => new CartItemVM
                {
                    ProductId = c.ProductId,
                    ProductName = c.Product.ProductName,
                    Quantity = c.Quantity,
                    Price = c.Product.Price,
                    ImagePath = c.Product.ImagePath

                })
                .ToList();
        }
        public Product? GetProductById(int productId)
        {
            return _context.Products.FirstOrDefault(u => u.Id == productId);
        }
        public void RemoveFromCart(int productId, Guid userId)
        {
            var cartItem = _context.Carts.FirstOrDefault(c => c.UserId == userId && c.ProductId == productId);
            if (cartItem != null)
            {
                _context.Carts.Remove(cartItem);
                _context.SaveChanges();
            }
        }
        public List<RecentlyViewedCookie> GetRecentlyViewedProducts(HttpRequest request)
        {
            var recentlyViewedJson = request.Cookies["RecentlyViewed"];
            var recentlyViewedProducts = new List<RecentlyViewedCookie>();

            if (!string.IsNullOrEmpty(recentlyViewedJson))
            {
                try
                {
                    recentlyViewedProducts = JsonSerializer.Deserialize<List<RecentlyViewedCookie>>(recentlyViewedJson);
                }
                catch (Exception)
                {
                    recentlyViewedProducts = new List<RecentlyViewedCookie>();
                }
            }

            return recentlyViewedProducts;
        }

        public void AddToRecentlyViewed(Product product, HttpResponse response)
        {
            var recentlyViewedProducts = GetRecentlyViewedProducts(response.HttpContext.Request);

            if (recentlyViewedProducts.All(p => p.ProductId != product.Id))
            {

                
                recentlyViewedProducts.Add(new RecentlyViewedCookie
                {
                    ProductId = product.Id,
                    ProductName = product.ProductName,
                    Description = product.Description,
                    Price =product.Price ,
                    ImagePath = product.ImagePath
                });

                var jsonData = JsonSerializer.Serialize(recentlyViewedProducts);
                response.Cookies.Append("RecentlyViewed", jsonData, new CookieOptions
                {
                    Expires = DateTimeOffset.Now.AddMinutes(30)
                });
            }
        }
        public void RemoveFromCookie(string cookieName, int productId, HttpRequest request, HttpResponse response)
        {
            var cookieData = request.Cookies[cookieName];
            if (!string.IsNullOrEmpty(cookieData))
            {
                var items = JsonSerializer.Deserialize<List<CartItemVM>>(cookieData);
                if (items != null)
                {
                    var itemToRemove = items.FirstOrDefault(item => item.ProductId == productId);
                    if (itemToRemove != null)
                    {
                        items.Remove(itemToRemove);
                        var updatedData = JsonSerializer.Serialize(items);
                        response.Cookies.Append(cookieName, updatedData, new CookieOptions
                        {
                            HttpOnly = true,
                            Expires = DateTime.Now.AddHours(1),
                            Path = "/"
                        });
                    }
                }
            }
        }
        public void AddToLikedItems(int likedId, HttpRequest request, HttpResponse response)
        {
            var item = _context.Products.FirstOrDefault(u => u.Id == likedId);
            if (item == null) return;

            var addInLikeJson = request.Cookies["AddInLike"];
            var addInLike = string.IsNullOrEmpty(addInLikeJson)
                ? new List<CartItemVM>()
                : JsonSerializer.Deserialize<List<CartItemVM>>(addInLikeJson);

            if (!addInLike.Any(like => like.ProductId == likedId))
            {
                addInLike.Add(new CartItemVM
                {
                    ProductId = item.Id,
                    ProductName = item.ProductName,
                    ImagePath = item.ImagePath,
                    Price = item.Price,
                });

                UpdateCookie("AddInLike", addInLike, response);
            }
        }

        public void AddToCart(int clickedId, int quantity, HttpRequest request, HttpResponse response, Guid? userId = null)
        {
            var item = _context.Products.FirstOrDefault(u => u.Id == clickedId);
            if (item == null) return;

            if (userId.HasValue)
            {
                var existingCartItem = _context.Carts
                    .FirstOrDefault(c => c.ProductId == clickedId && c.UserId == userId);

                if (existingCartItem == null)
                {
                    var newCartItem = new Cart
                    {
                        UserId = userId.Value,
                        ProductId = item.Id,
                        Quantity = quantity 
                    };

                    _context.Carts.Add(newCartItem);
                }
                else
                {
                    existingCartItem.Quantity += quantity; 
                }

                _context.SaveChanges();
            }
            else
            {
                var addInCartJson = request.Cookies["AddInCart"];
                var addInCart = string.IsNullOrEmpty(addInCartJson)
                    ? new List<CartItemVM>()
                    : JsonSerializer.Deserialize<List<CartItemVM>>(addInCartJson);

                var existingCartItem = addInCart.FirstOrDefault(cart => cart.ProductId == clickedId);
                if (existingCartItem != null)
                {
                    existingCartItem.Quantity += quantity; 
                }
                else
                {
                    addInCart.Add(new CartItemVM
                    {
                        ProductId = item.Id,
                        ProductName = item.ProductName,
                        ImagePath = item.ImagePath,
                        Price = item.Price,
                        Quantity = quantity 
                    });
                }

                UpdateCookie("AddInCart", addInCart, response);
            }
        }

        public void UpdateCookie(string cookieName, List<CartItemVM> items, HttpResponse response)
        {
            var jsonData = JsonSerializer.Serialize(items);
            response.Cookies.Append(cookieName, jsonData, new CookieOptions
            {
                Expires = DateTimeOffset.Now.AddDays(1)
            });
        }


    }

}
using Microsoft.AspNetCore.Mvc;
using Shopily.Domain.Entity;
using Shopily.Repositories;
using System.Security.Claims;
using System.Text.Json;
using Stripe;
using Stripe.Checkout;
using Microsoft.Extensions.Options;
using Shopily.Services;
using Shopily.Data;
using Shopily.Domain.Models;
using Shopily.Domain.ViewModel.Cart;
using Shopily.Domain.ViewModel.Orders;
using MimeKit;
using Shopily.Domain.ViewModel;
using MailKit.Net.Smtp;
using static System.Net.Mime.MediaTypeNames;

namespace Shopily.Controllers
{
    public class ShopController : Controller
    {
        private readonly ShopRepository _shopRepository;
        private readonly Context _context;
        private readonly StripeSettings _stripeSettings;
        public ShopController(ShopRepository shopRepository, Context context, IOptions<StripeSettings> stripeSetting)
        {
            _shopRepository = shopRepository; 
            _context = context;  
            _stripeSettings = stripeSetting.Value; 
        }



        [Route("Liked")]
        public IActionResult Like(int page = 1)
        {
            var recentlyViewedJson = Request.Cookies["AddInLike"];
            List<CartItemVM> recentlyViewedProducts = string.IsNullOrEmpty(recentlyViewedJson)
                ? new List<CartItemVM>()
                : JsonSerializer.Deserialize<List<CartItemVM>>(recentlyViewedJson) ?? new List<CartItemVM>();

            int itemsPerPage = 12;
            var products = _shopRepository.GetProducts(page, itemsPerPage);
            var totalProducts = _shopRepository.GetTotalProductsCount();
            var pagesCount = (int)Math.Ceiling((double)totalProducts / itemsPerPage);
            ViewData["AddInLike"] = recentlyViewedProducts;
            return View();
        }

        [Route("Cart")]
        public IActionResult Cart(int page = 1)
        {
            if (!User.Identity.IsAuthenticated)
            {
                var recentlyViewedJson = Request.Cookies["AddInCart"];
                List<CartItemVM> recentlyViewedProducts = string.IsNullOrEmpty(recentlyViewedJson)
                    ? new List<CartItemVM>()
                    : JsonSerializer.Deserialize<List<CartItemVM>>(recentlyViewedJson) ?? new List<CartItemVM>();

                ViewData["AddInCart"] = recentlyViewedProducts;
                return View();
            }
            else
            {
                var loggedUserId = Guid.Parse(User.FindFirst(ClaimTypes.Sid)?.Value);
                var cartItems = _shopRepository.GetCartItems(loggedUserId);

                return View(new IndexVM { Items = cartItems });
            }
        }
        [Route("Shop")]
        public IActionResult Shop(string productName, string category, string productTypes, int page = 1)
        {
            string[] productTypesArray = string.IsNullOrEmpty(productTypes) ? new string[] { } : productTypes.Split(',');

            var shopVM = _shopRepository.CreateShopVM(productName, category, productTypesArray, page);
            return View(shopVM);
        }
        public IActionResult Delete(int productId, int likedId)
        {
            if (likedId != 0)
            {
                _shopRepository.RemoveFromCookie("AddInLike", likedId, Request, Response);
                TempData["Notification"] = "Item removed from Liked!";
                TempData["NotificationType"] = "warning";
            }
            else if (User.Identity.IsAuthenticated)
            {
                var loggedUserId = Guid.Parse(User.FindFirst(ClaimTypes.Sid)?.Value);
                _shopRepository.RemoveFromCart(productId, loggedUserId);
                TempData["Notification"] = "Item removed from Cart!";
                TempData["NotificationType"] = "warning";
            }
            else
            {
                _shopRepository.RemoveFromCookie("AddInCart", productId, Request, Response);
                TempData["Notification"] = "Item removed from Cart!";
                TempData["NotificationType"] = "warning";
            }

            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult Shopsingle(int productId)
        {
            var product = _shopRepository.GetProductById(productId);
            if (product == null)
            {
                return RedirectToAction("Index", "Home"); 
            }

            _shopRepository.AddToRecentlyViewed(product, Response);

            var recentlyViewed = Request.Cookies["RecentlyViewed"];
            ViewBag.RecentlyViewed = recentlyViewed;

            return View(ProductServices.AddSigneProduct(product));
        }
        [HttpPost]
        [Route("Shop/AddInCart")]
        public IActionResult AddInCart(int clickedId, int? likedId, int quantity = 1)
        {
            if (likedId.HasValue && likedId.Value != 0)
            {
                var addInLikeJson = Request.Cookies["AddInLike"];
                var addInLike = string.IsNullOrEmpty(addInLikeJson)
                    ? new List<CartItemVM>()
                    : JsonSerializer.Deserialize<List<CartItemVM>>(addInLikeJson);

                if (addInLike.Any(like => like.ProductId == likedId.Value))
                {
                    TempData["Notification"] = "Product is already in liked items!";
                    TempData["NotificationType"] = "info";
                }
                else
                {
                    _shopRepository.AddToLikedItems(likedId.Value, Request, Response);
                    TempData["Notification"] = "Product added to liked items!";
                    TempData["NotificationType"] = "success";
                }
            }
            else
            {
                var item = _context.Products.FirstOrDefault(u => u.Id == clickedId);
                if (item == null)
                {
                    TempData["Notification"] = "Product not found!";
                    TempData["NotificationType"] = "error";
                    return RedirectToAction("Shopsingle", new { ProductId = clickedId });
                }

                if (User.Identity.IsAuthenticated)
                {
                    var loggedUserId = Guid.Parse(User.FindFirst(ClaimTypes.Sid)?.Value);
                    var existingCartItem = _context.Carts.FirstOrDefault(c => c.ProductId == clickedId && c.UserId == loggedUserId);

                    if (existingCartItem != null)
                    {
                        TempData["Notification"] = "Product is already in the cart!";
                        TempData["NotificationType"] = "info";
                    }
                    else
                    {
                        _context.Carts.Add(new Cart
                        {
                            UserId = loggedUserId,
                            ProductId = item.Id,
                            Quantity = quantity
                        });
                        _context.SaveChanges();

                        TempData["Notification"] = "Product added to cart!";
                        TempData["NotificationType"] = "success";
                    }
                }
                else
                {
                    var addInCartJson = Request.Cookies["AddInCart"];
                    var addInCart = string.IsNullOrEmpty(addInCartJson)
                        ? new List<CartItemVM>()
                        : JsonSerializer.Deserialize<List<CartItemVM>>(addInCartJson);

                    if (addInCart.Any(cart => cart.ProductId == clickedId))
                    {
                        TempData["Notification"] = "Product is already in the cart!";
                        TempData["NotificationType"] = "info";
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

                        _shopRepository.UpdateCookie("AddInCart", addInCart, Response);
                        TempData["Notification"] = "Product added to cart!";
                        TempData["NotificationType"] = "success";
                    }
                }
            }

            return RedirectToAction("Shopsingle", new { ProductId = clickedId });
        }

     public IActionResult CreateCheckOutSession(OrderVM order)
        {

            var currency = "usd";
            var cancelUrl = "https://mediathrive.com/";  

            StripeConfiguration.ApiKey = _stripeSettings.SecretKey;

            // Collect ProductIds from the cart items

            var options = new Stripe.Checkout.SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
        {
            new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    Currency = currency,
                    UnitAmount = (long)(order.TotalCost * 100),
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = "Total Cart Purchase",
                        Description = "Total amount for cart"
                    }
                },
              Quantity=1
            }
        },
                Mode = "payment",
                SuccessUrl = $"{Request.Scheme}://{Request.Host}" + Url.Action("CheckoutSuccess", "Shop", order),  
                CancelUrl = cancelUrl,
                Metadata = new Dictionary<string, string>
        {
            { "OrderId", $"{order.Id}" },
            { "CustomerName", $"{order.FirstName} {order.LastName}" },
            { "CustomerEmail", $"{order.Email}" },
            { "CustomerPhone", $"{order.Phone}" },
            { "CustomerCountry", $"{order.Country}" },
            { "TotalCost", $"{order.TotalCost}" },
            { "OrderDate", $"{order.OrderDate:yyyy-MM-dd}" },
            { "ProductIds", order.ProductIds },
                    {"Status",order.Status },
                    {"Quantity",order.ProductQuantity }

        }
            };

            var service = new Stripe.Checkout.SessionService();
            var session = service.Create(options);

            return Redirect(session.Url);
        }

        [HttpGet]
        public IActionResult Checkout()
        {
            if (User.Identity.IsAuthenticated)
            {
                var loggedUserId = Guid.Parse(User.FindFirst(ClaimTypes.Sid)?.Value);
                var cartItems = _shopRepository.GetCartItems(loggedUserId);

                OrderVM order = new OrderVM(loggedUserId, cartItems);

                return View(order);
            }
            return RedirectToAction("Cart");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Checkout(OrderVM order)
        {      
                return CreateCheckOutSession(order);                      
        }

        public async Task<IActionResult> CheckoutSuccess(OrderVM order)
        {

            var orderRepository = new OrderRepositoy(_context);
            orderRepository.SaveOrder(new Order(order));

            var productIds = order.ProductIds.Split(',').Select(int.Parse).ToList();
            var products = _context.Products.Where(p => productIds.Contains(p.Id)).ToList();
            var QuantityId = order.ProductQuantity.Split(',').Select(int.Parse).ToList();

            var bodyBuilder = new BodyBuilder();
            var cartItemsHtml = "";

            foreach (var product in products)
            {
                string contentId = Guid.NewGuid().ToString(); 
                var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/", product.ImagePath);

                if (System.IO.File.Exists(imagePath))
                {
                    var image = bodyBuilder.LinkedResources.Add(imagePath);
                    image.ContentId = contentId;
                }

                var index = productIds.IndexOf(product.Id);
                var quantity = (index >= 0) ? QuantityId[index] : 0;

                cartItemsHtml += $@"
            <tr>
                <td style='border: 1px solid #ddd; padding: 8px; text-align: center;'>
                    <img src='cid:{contentId}' style='width: 50px; height: auto;' />
                </td>
                <td style='border: 1px solid #ddd; padding: 8px;'>{product.ProductName}</td>
                <td style='border: 1px solid #ddd; padding: 8px;'>{product.Id}</td>
                <td style='border: 1px solid #ddd; padding: 8px;'>{quantity}</td> <!-- Correct quantity -->
                <td style='border: 1px solid #ddd; padding: 8px;'>${product.Price.ToString("F2")}</td>
                <td style='border: 1px solid #ddd; padding: 8px;'>${(product.Price * quantity).ToString("F2")}</td> <!-- Correct total -->
            </tr>";
            }

            bodyBuilder.HtmlBody = $@"
    <div style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
        <h2 style='color: #4CAF50;'>Thank you for your order!</h2>
        <p>Dear <strong>{order.FirstName} {order.LastName}</strong>,</p>
        <p>We have successfully received your order. Here are the details:</p>
        <table style='width: 100%; border-collapse: collapse; margin: 20px 0;'>
            <thead>
                <tr style='background: #f4f4f4;'>
                    <th style='border: 1px solid #ddd; padding: 8px; text-align: left;'>Product Image</th>
                    <th style='border: 1px solid #ddd; padding: 8px; text-align: left;'>Product Name</th>
                    <th style='border: 1px solid #ddd; padding: 8px; text-align: left;'>Product ID</th>
                    <th style='border: 1px solid #ddd; padding: 8px; text-align: left;'>Quantity</th>
                    <th style='border: 1px solid #ddd; padding: 8px; text-align: left;'>Price</th>
                    <th style='border: 1px solid #ddd; padding: 8px; text-align: left;'>Total</th>
                </tr>
            </thead>
            <tbody>

                {cartItemsHtml}
            </tbody>
            <tfoot>
                <tr style='background: #f9f9f9; font-weight: bold;'>
                    <td colspan='5' style='border: 1px solid #ddd; padding: 8px; text-align: right;'>Total Cost:</td>
                    <td style='border: 1px solid #ddd; padding: 8px;'>${order.TotalCost.ToString("F2")}</td>
                </tr>
              
            </tfoot>
        </table>
  <p style='margin-top: 20px;'>This is the notes that you left for your order {order.OrderNotes}</p>
        <p style='margin-top: 20px;'>If you have any questions, feel free to contact us at <a href='mailto:support@shopily.com'>support@shopily.com</a>.</p>
        <p style='margin-top: 30px;'>Best regards,<br/><strong>The Shopily Team</strong></p>
        <hr />
        <p style='font-size: 12px; color: #999;'>This is an automated email. Please do not reply to this email.</p>
    </div>";

            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("Shopily", "noreply@shopily.com"));
            emailMessage.To.Add(new MailboxAddress("", order.Email));
            emailMessage.Subject = "Your Shopily Order Confirmation";
            emailMessage.Body = bodyBuilder.ToMessageBody(); 

            using (var smtp = new SmtpClient())
            {
                await smtp.ConnectAsync("smtp.mailtrap.io", 587, MailKit.Security.SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync("cd8148ee8ed88d", "56907250c2b5cb");
                await smtp.SendAsync(emailMessage);
                await smtp.DisconnectAsync(true);
            }

            TempData["Notification"] = "Your order confirmation email has been sent successfully!";
            return RedirectToAction("Thankyou", "Home");
        }

    }
}



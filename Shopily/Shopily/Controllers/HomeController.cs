using Microsoft.AspNetCore.Mvc;
using Shopily.Data;
using System.Text.Json;
using System.Globalization;
using Shopily.Domain.ViewModel.Products;
using MimeKit;
using Shopily.Domain.ViewModel;
using MailKit.Net.Smtp;

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
                   
                    recentlyViewedProducts = new List<RecentlyViewedCookie>();
                }
            }
            ViewData["RecentlyViewed"] = recentlyViewedProducts;
           
            return View(model);
           
        }
        [Route("About")]
        public IActionResult About()
        {
            return View();
        }
        [HttpGet]
        [Route("Contact")]
        public IActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        [Route("Home/SendMailFromLayout")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendMailFromLayout(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("Email is required.");
            }

            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("Shopily", "noreply@shopily.com"));
            emailMessage.To.Add(new MailboxAddress("", email));  
            emailMessage.Subject = "Sended by Shopily";

            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = $@"
        <div style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
            <h2 style='color: #4CAF50;'>Hello,</h2>
            <p>Thank you for subscribing to our updates. We're thrilled to have you with us!</p>
            <p style='margin: 20px 0;'>Stay tuned for exciting news and updates from <strong>Shopily</strong>.</p>
            <p style='color: #999;'>If you have any questions, feel free to reach out to us at <a href='mailto:support@shopily.com'>support@shopily.com</a>.</p>
            <p style='margin-top: 30px;'>Best regards,<br/><strong>The Shopily Team</strong></p>
        </div>"
            };

            using (var smtp = new SmtpClient())
            {
                smtp.Connect("smtp.mailtrap.io", 587, MailKit.Security.SecureSocketOptions.StartTls);
                smtp.Authenticate("cd8148ee8ed88d", "56907250c2b5cb"); 

                await smtp.SendAsync(emailMessage);
                smtp.Disconnect(true);
            }

 
            TempData["Notification"] = "Email sent successfully!";
            return RedirectToAction("Index", "Home");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendMail(ContactVM model)
        {
            if (!ModelState.IsValid)
            {
                return View("Contact", model); 
            }

            try
            {
            
                var emailMessage = new MimeMessage();
                emailMessage.From.Add(new MailboxAddress("Shopily", "noreply@shopily.com"));
                emailMessage.To.Add(new MailboxAddress(@model.Name,model.Email)); 
                emailMessage.Subject = model.Subject;

        
                emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
                {
                    Text = $@"
        <div style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
            <h2 style='color: #4CAF50;'>New Contact Request</h2>
            <p><strong>Name:</strong> {model.Name}</p>
            <p><strong>Email:</strong> {model.Email}</p>
            <p><strong>Phone:</strong> {model.PhoneNumber}</p>
            <p><strong>Subject:</strong> {model.Subject}</p>
            <p><strong>Message:</strong></p>
            <blockquote style='margin: 20px 0; padding: 15px; background: #f9f9f9; border-left: 4px solid #4CAF50;'>
                {model.Message}
            </blockquote>
            <p style='margin-top: 30px;'>You can reply to this email to get in touch with the sender.</p>
            <p style='color: #999;'>This is an automated email sent from the Shopily contact form.</p>
        </div>"
                };


        
                using (var smtp = new SmtpClient())
                {
                    smtp.Connect("smtp.mailtrap.io", 587, MailKit.Security.SecureSocketOptions.StartTls);
                    smtp.Authenticate("cd8148ee8ed88d", "56907250c2b5cb"); 
                    await smtp.SendAsync(emailMessage);
                    smtp.Disconnect(true);
                }

                TempData["Notification"] = "Your message has been sent successfully!";
                TempData["NotificationType"] = "success";
                return RedirectToAction("Contact");
            }
            catch (Exception ex)
            {
             
                Console.WriteLine($"Error sending email: {ex.Message}");

                TempData["Error"] = "There was an error sending your message. Please try again later.";
                TempData["NotificationType"] = "error";
                return RedirectToAction("Contact");
            }
        }

        [Route("Thankyou")]
        public IActionResult Thankyou()
        {
            return View();
        }
        public IActionResult ChangeLanguage(string lang)
        {
            if (!string.IsNullOrEmpty(lang))
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(lang);
                Thread.CurrentThread.CurrentUICulture=new CultureInfo(lang);

            }
            else
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en");
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("en");
                lang = "en";

            }
            Response.Cookies.Append("Language", lang);
            return Redirect(Request.GetTypedHeaders().Referer.ToString());
        }
        


    }
}

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Shopily.Repositories;
using System.Security.Claims;
using Shopily.Services;
using Shopily.Data;
using Shopily.Domain.Entity;
using Shopily.Domain.ViewModel.User;
using MimeKit;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace Shopily.Controllers
{

    public class UserController : Controller
    {
        private readonly Context context;
        private readonly UserRepository _userRepository;
        public UserController(Context _context, UserRepository userRepository)
        {
            context = _context;
            _userRepository = userRepository;
        }
        public IActionResult Index()
        {
            return View();
        }
        [Route("Login")]
        public IActionResult Login()
        {
            return View();
        }
        [HttpGet]
        [Route("Register")]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]  
        [Route("Register")]
        public async Task<IActionResult> RegisterAsync(RegisterVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model); 
            }

            var verificationCode = new Random().Next(100000, 999999).ToString();

            HttpContext.Session.SetString("FirstName", model.FirstName);
            HttpContext.Session.SetString("LastName", model.LastName);
            HttpContext.Session.SetString("Username", model.Username);
            HttpContext.Session.SetString("Email", model.Email);
            HttpContext.Session.SetString("Password", model.Password); 

            HttpContext.Session.SetString("VerificationCode", verificationCode);

            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("Shopily", "noreply@shopily.com"));
            emailMessage.To.Add(new MailboxAddress("", model.Email));
            emailMessage.Subject = "Verification Code from Shopily";

            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = $@"
<div style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
    <h2 style='color: #4CAF50;'>Hello,</h2>
    <p>Your verification code is <strong>{verificationCode}</strong>.</p>
    <p>Please enter this code to complete your registration.</p>
    <p style='color: #999;'>If you didn't request this, please ignore this email.</p>
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
            TempData["SuccessMessage"] = "Registration successful!";
            return RedirectToAction("Verify", "User");
        }

        public IActionResult Verify()
        {
            return View(); 
        }

        [HttpPost]
        [ValidateAntiForgeryToken] 

        public async Task<IActionResult> Verify(string verificationCode)
        {
            var storedCode = HttpContext.Session.GetString("VerificationCode");

            if (verificationCode == storedCode)
            {

                var firstName = HttpContext.Session.GetString("FirstName");
                var lastName = HttpContext.Session.GetString("LastName");
                var username = HttpContext.Session.GetString("Username");
                var email = HttpContext.Session.GetString("Email");
                var password = HttpContext.Session.GetString("Password");


                var registerVM = new RegisterVM
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Username = username,
                    Email = email,
                    Password = password
                };
               _userRepository.RegisterUserVM(registerVM);

                TempData["RegistrationComplete"] = "Registration complete!";
                TempData["NotificationType"] = "success";
                HttpContext.Session.Clear();              
                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["VerificationError"] = "The verification code is incorrect.";
                TempData["NotificationType"] = "success";
                ModelState.AddModelError("", "Invalid verification code.");
                return View();
            }
        }

        public IActionResult Login(LoginVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            User? logIn = context.Users
                .FirstOrDefault(u => u.Username == model.Username && u.Password == model.Password);

            if (logIn == null)
            {

                ModelState.AddModelError(string.Empty, "Invalid username or password.");
                return View(model);
            }

            List<Claim> claims = new List<Claim>() {
                new Claim(ClaimTypes.NameIdentifier , $"{logIn.Username}"),
                new Claim(ClaimTypes.Name, logIn.Username),
                new Claim(ClaimTypes.Role , logIn.IsAdmin ? "Admin": "User"),
                new Claim(ClaimTypes.Sid , logIn.Id.ToString())
                };

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            AuthenticationProperties properties = new AuthenticationProperties()
            {
                AllowRefresh = true,

            };

            HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity), properties);



            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Route("Edit")]
        public IActionResult Edit()
        {
            
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login");
            }

            var userIdString = HttpContext.User.FindFirst(ClaimTypes.Sid)?.Value;
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid userId))
            {
                return RedirectToAction("Home");
            }

            User? item = context.Users.SingleOrDefault(u => u.Id == userId);
            if (item == null)
            {
                return RedirectToAction("Home");
            }

          return View(UserServices.createEditVM(item));
        }

        [HttpPost]
        public IActionResult Edit(EditVM model)
        {

            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login");
            }

            var userIdString = HttpContext.User.FindFirst(ClaimTypes.Sid)?.Value;
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid userId))
            {
                return RedirectToAction("Index");
            }

            User? item = context.Users.SingleOrDefault(u => u.Id == userId);
            if (item == null)
            {
                return RedirectToAction("Index");
            }

            if (model.OldPassword != item.Password)
            {
                ViewData["PasswordError"] = "The old password is incorrect.";
                return View(model);
            }

            if (!string.IsNullOrEmpty(model.Password))
            {
                item.Password = model.Password;
            }

            item.EditUser(model);

            context.Users.Update(item);
            context.SaveChanges();

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Index", "Home");
        }
       
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordVM model)
        {
            if (ModelState.IsValid)
            {
                var user = await context.Users
                    .FirstOrDefaultAsync(u => u.Username == model.Username && u.Email == model.Email);

                if (user != null)
                {
                    var token = Guid.NewGuid().ToString();

                    HttpContext.Session.SetString("ResetToken", token);
                    HttpContext.Session.SetString("ResetUserId", user.Id.ToString());

                    var resetLink = Url.Action("ResetPassword", "User", new { token }, Request.Scheme);

                    var emailMessage = new MimeMessage();
                    emailMessage.From.Add(new MailboxAddress("Shopily", "noreply@yourapp.com"));
                    emailMessage.To.Add(new MailboxAddress("", model.Email));
                    emailMessage.Subject = "Password Reset Request";
                    emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
                    {
                        Text = $@"
                <div>
                    <p>Click the link below to reset your password:</p>
                    <a href='{resetLink}'>Reset Password</a>
                </div>"
                    };

                    try
                    {
                        using (var smtp = new SmtpClient())
                        {
                            smtp.Connect("smtp.mailtrap.io", 587, MailKit.Security.SecureSocketOptions.StartTls);
                            smtp.Authenticate("cd8148ee8ed88d", "56907250c2b5cb");
                            await smtp.SendAsync(emailMessage);
                            smtp.Disconnect(true);
                        }

                        TempData["Success"] = "Password reset link has been sent to your email.";
                    }
                    catch (Exception ex)
                    {
                        TempData["Error"] = "An error occurred while sending the reset email. Please try again.";
                        
                    }

                    return RedirectToAction("ForgotPassword");
                }

                TempData["Error"] = "Invalid username or email.";
            }

            return View(model);
        }



        [HttpGet]
        public IActionResult ResetPassword(string token)
        {
            var sessionToken = HttpContext.Session.GetString("ResetToken");

            if (string.IsNullOrEmpty(sessionToken) || sessionToken != token)
            {
                TempData["Error"] = "Invalid or expired reset token.";
                return RedirectToAction("ForgotPassword");
            }

            return View(new ResetPasswordVM { Token = token });
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM model)
        {
            if (ModelState.IsValid)
            {
                var sessionToken = HttpContext.Session.GetString("ResetToken");
                var sessionUserId = HttpContext.Session.GetString("ResetUserId");

                if (string.IsNullOrEmpty(sessionToken) || sessionToken != model.Token || string.IsNullOrEmpty(sessionUserId))
                {
                    TempData["Error"] = "Invalid or expired reset token.";
                    return RedirectToAction("ForgotPassword");
                }

              
                var userId = Guid.Parse(sessionUserId);
                var user = await context.Users.FindAsync(userId);

                if (user != null)
                {

                    user.Password = model.NewPassword;
                    await context.SaveChangesAsync();

                    HttpContext.Session.Remove("ResetToken");
                    HttpContext.Session.Remove("ResetUserId");

                    TempData["Success"] = "Password has been reset successfully.";
                    return RedirectToAction("Login");
                }

                TempData["Error"] = "User not found.";
            }

            return View(model);
        }
    }

    }



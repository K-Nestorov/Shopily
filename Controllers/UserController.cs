using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Shopily.Entity;
using Shopily.Repositories;
using Shopily.ViewModel.User;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace Shopily.Controllers
{
    public class UserController : Controller
    {
        private readonly Context context;

        public UserController(Context _context)
        {
            context = _context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Route("Login")]
        public IActionResult Login()
        {
            LoginVM model=new LoginVM();
            if (model.Username == null)
            {
                return View(model);
            }
            else
            {
                List<Claim> claims = new List<Claim>() {
                    new Claim(ClaimTypes.NameIdentifier , $"{model.Username}"),
                    
                    new Claim(ClaimTypes.Name, model.Username),
                    new Claim(ClaimTypes.Role , model.isAdmin ? "Admin": "User"),
                    new Claim(ClaimTypes.Sid , model.Id.ToString())
                };

                ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

                AuthenticationProperties properties = new AuthenticationProperties()
                {
                    AllowRefresh = true,
                   
                };

                HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity), properties);
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Route("Register")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(RegisterVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            User item = new User();
            item.RegisterUser(model);

            context.Users.Add(item);
            context.SaveChanges();

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM model)
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

            
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, logIn.Username),
                new Claim(ClaimTypes.Name, logIn.Username),
                new Claim(ClaimTypes.Email, logIn.Email),
                new Claim(ClaimTypes.Role, logIn.IsAdmin ? "Admin" : "User"),
                new Claim(ClaimTypes.Sid, logIn.Id.ToString())
            };
          
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var properties = new AuthenticationProperties
            {
                AllowRefresh = true,
                IsPersistent = model.RememberMe 
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity), properties);

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Route("Edit")]
        public IActionResult Edit()
        {
            var loggedUser = User; 

            if (loggedUser == null)
            {
                return RedirectToAction("Login");
            }

            var userId = loggedUser.FindFirst(ClaimTypes.Sid)?.Value;
            var item = context.Users.SingleOrDefault(u => u.Id.ToString() == userId);

            if (item == null)
            {
                return RedirectToAction("Home");
            }

            EditVM model = new EditVM
            {
                Id = item.Id,
                Username = item.Username,
                FirstName = item.FirstName,
                LastName = item.LastName,
                Email = item.Email,
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(EditVM model)
        {
            var loggedUser = User; 

            if (loggedUser == null)
            {
                return RedirectToAction("Login");
            }

            var userId = loggedUser.FindFirst(ClaimTypes.Sid)?.Value;
            var item = context.Users.SingleOrDefault(u => u.Id.ToString() == userId);

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
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}

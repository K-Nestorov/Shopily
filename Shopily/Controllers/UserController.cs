using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Shopily.Entity;
using Shopily.Repositories;
using Shopily.ViewModel.User;
using System.Security.Claims;
using Shopily.Cookies;
using Microsoft.EntityFrameworkCore;

namespace Shopily.Controllers
{
    public class UserController : Controller
    {
        private readonly Context context;

        // Constructor for dependency injection
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
            return View();
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
                return View(model);  // If model is invalid, return back to the view with validation errors
            }

            User item = new User();
            item.RegisterUser(model);  // Assuming this method maps the RegisterVM to the User model



            context.Users.Add(item);
            context.SaveChanges();  
            Response.Cookies.SetObject("UserInfo", item, 30);

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Login(LoginVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Find the user based on username and password
            User? logIn = context.Users
                .FirstOrDefault(u => u.Username == model.Username && u.Password == model.Password);

            if (logIn == null)
            {
                // If login fails, return the view with the model
                ModelState.AddModelError(string.Empty, "Invalid username or password.");
                return View(model);
            }

            // Create user data to store in a cookie
            var userCookieData = new
            {
                Id = logIn.Id,
                Username = logIn.Username,
                Email = logIn.Email,
                IsAdmin = logIn.IsAdmin
            };

            // Set the user information as a cookie
            Response.Cookies.SetObject("UserInfo", userCookieData, expireTimeInMinutes: 60);

            // Redirect to the home page or appropriate action
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Route("Edit")]
        public IActionResult Edit()
        {
            // Retrieve the logged-in user from the cookie
            User loggedUser = Request.Cookies.GetObject<User>("UserInfo");

            // If no logged-in user found, redirect to login
            if (loggedUser == null)
            {
                return RedirectToAction("Login");
            }

            // Retrieve the user details from the database
            User? item = context.Users.SingleOrDefault(u => u.Id == loggedUser.Id);

            // If no user found in the database, redirect to the home page
            if (item == null)
            {
                return RedirectToAction("Home");
            }

            // Populate the EditVM model with the user data
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
          

            User loggedUser = Request.Cookies.GetObject<User>("UserInfo");

            if (loggedUser == null)
            {
                return RedirectToAction("Login");
            }

            User? item = context.Users.SingleOrDefault(u => u.Id == loggedUser.Id);

            if (item == null)
            {
                return RedirectToAction("Index");
            }

            if (model.OldPassword != item.Password)
            {
                ViewData["PasswordError"] = "The old password is incorrect.";
                return View(model);
            }

            // Update the fields from the model
            if (!string.IsNullOrEmpty(model.Password))
            {
                item.Password = model.Password;
            }


            item.EditUser(model);
            context.Users.Update(item);
            context.SaveChanges();

            return RedirectToAction("Index","Home");
        }
        public IActionResult Logout()
        {
            Response.Cookies.Delete("UserInfo");
            return RedirectToAction("Index", "Home");
        }

    }
}


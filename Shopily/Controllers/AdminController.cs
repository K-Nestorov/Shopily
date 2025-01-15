using Microsoft.AspNetCore.Mvc;
using Shopily.Entity;
using Shopily.Repositories;
using Shopily.ViewModel.Admin;
using Shopily.ViewModel.Products;
using Shopily.ViewModel.User;


namespace Shopily.Controllers
{
    public class AdminController : Controller
    {
        private readonly Context context;

        // Constructor for dependency injection
        public AdminController(Context _context)
        {
            context = _context;
        }
        public IActionResult Index(AdminIndexVm model)
        {
            model.Items=context.Users.ToList();
            return View(model);
        }
        [HttpGet]
        [Route("CreateUser")]
        public IActionResult CreateUser()
        {
            return View(new RegisterVM());
        }
        [HttpPost]
        public IActionResult CreateUser(RegisterVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            User item = new User(model);

            context.Users.Add(item);
            context.SaveChanges();

            return RedirectToAction("Index", "Home");
        }
        [HttpGet]

        [HttpGet]
        public IActionResult AdminEdit(Guid id) // Ensure 'id' is Guid
        {
            // Find the user by Guid
            User? item = context.Users.FirstOrDefault(u => u.Id == id);
            if (item == null)
            {
                return RedirectToAction("Index", "Admin");
            }

            AdminEditVM model = new AdminEditVM
            {
                Id = item.Id, 
                Username = item.Username,
                FirstName = item.FirstName,
                LastName = item.LastName,
                Email = item.Email,
                IsAdmin = item.IsAdmin,
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult AdminEdit(AdminEditVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Find the user by Guid
            User? item = context.Users.FirstOrDefault(u => u.Id == model.Id);
            if (item == null)
            {
                return RedirectToAction("Index", "Admin");
            }
            item.AdminEditUser(model);
            context.Users.Update(item);
            context.SaveChanges();

            return RedirectToAction("Index", "Admin");
        }
        public IActionResult Delete(Guid id)
        {
            User item = new User();
            item.Id = id;
            Cart cart = new Cart();
            context.Users.Remove(item);
            context.SaveChanges();

            return RedirectToAction("Index", "Admin");
        }
        [HttpGet]
        [Route("AdminProductEdit")]
        public IActionResult AdminProductEdit(int id)
        {
            Product? item = context.Products.Where(u => u.Id == id).FirstOrDefault();
            if (item == null)
            {
                return RedirectToAction("AdminProduct", "Admin");

            }
            ViewModel.Products.EditVM model = new ViewModel.Products.EditVM()
            {
                Id = item.Id,
                ProductName = item.ProductName,
                Price = item.Price,
                Description = item.Description,
                ImagePath = item.ImagePath,
                Gender = item.Gender,
                Type = item.Type,
            };
            return View(model);


        }
        [HttpPost]
        public IActionResult AdminProductEdit(ViewModel.Products.EditVM model)
        {

            if (!ModelState.IsValid)
                return View(model);

            Product item = new Product(model);


            context.Products.Update(item);
            context.SaveChanges();

            return RedirectToAction("AdminProduct", "Admin");
        }









    }
}

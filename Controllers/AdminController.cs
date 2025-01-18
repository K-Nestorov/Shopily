using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Shopily.Entity;
using Shopily.Repositories;
using Shopily.ViewModel.Admin;
using Shopily.ViewModel.Products;
using Shopily.ViewModel.User;


namespace Shopily.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IWebHostEnvironment _env;

        private readonly Context context;

        
        public AdminController(Context _context, IWebHostEnvironment env)
        {
            _env = env;
            context = _context;
        }
        public IActionResult Index(AdminIndexVm model)
        {
            model.Items=context.Users.ToList();
            return View(model);
        }
        [HttpGet]
        [Route("AdminProduct")]
        public IActionResult AdminProduct(IndexVM model)
        {
            model.Items =context.Products.ToList();
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
        public IActionResult AdminEdit(Guid id) 
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

        public IActionResult AdminProductDelete(int id)
        {
            Product item = context.Products.Find(id);
          
                string imagePath = item.ImagePath; 
                if (!string.IsNullOrEmpty(imagePath) && System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath); 
                }

                context.Products.Remove(item);
                context.SaveChanges();
            
            return RedirectToAction("AdminProduct", "Admin");
        }



        [HttpGet]
        [Route("ProductCreate")]
        public IActionResult AdminProductCreate()
        {

            return View(new CreateVM());
        }
        [HttpPost]
        public IActionResult AdminProductCreate(CreateVM model)
        {
            if (!ModelState.IsValid)
                return View(model);


            string fileName = null;
            if (model.ImagePath != null)
            {
                string uploadsFolder = Path.Combine(_env.WebRootPath, "Images");
                fileName = Guid.NewGuid().ToString() + "_" + model.ImagePath.FileName;
                string filePath = Path.Combine(uploadsFolder, fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.ImagePath.CopyTo(fileStream);
                }
            }


            Product item = new Product
            {
                Description = model.Description,
                Price = model.Price,
                ProductName = model.ProductName,
                Gender = model.Gender,
                WareHouseQuantity = model.WareHouseQuantity,
               Type = model.Type,             
                TimeOfAdd=DateTime.Now,              
                ImagePath = fileName
                
            };
            item.ProductCreate(model);
            context.Products.Add(item);
            context.SaveChanges();

            return RedirectToAction("AdminProduct", "Admin");
        }






    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Shopily.Data;
using Shopily.Domain.Entity;
using Shopily.Domain.ViewModel.Admin;
using Shopily.Domain.ViewModel.Orders;
using Shopily.Domain.ViewModel.Products;
using Shopily.Domain.ViewModel.User;
using Shopily.Repositories;
using Shopily.Services;

namespace Shopily.Controllers
{
 

    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly AdminRepository _adminRepository;
        private readonly IWebHostEnvironment _env;
        private readonly Context _context;

        public AdminController(AdminRepository adminRepository, IWebHostEnvironment env, Context context)
        {
            _adminRepository = adminRepository;
            _env = env;
            _context = context;
        }

        public IActionResult Index(AdminIndexVm model)
        {
            model.Items = _adminRepository.GetAllUsers();
            return View(model);
        }

        [HttpGet]
        [Route("AdminProduct")]
        public IActionResult AdminProduct(IndexVM model)
        {
            model.Items = _adminRepository.GetAllProducts();
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
            _adminRepository.AddUser(item);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult AdminEdit(Guid id)
        {
            User item = _adminRepository.GetUserById(id);

            if (item == null)
            {
                return RedirectToAction("Index", "Admin");
            }

            return View(AdminServices.CreateAdminEditVM(item));
        }

        [HttpPost]
        public IActionResult AdminEdit(AdminEditVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            User item = _adminRepository.GetUserById(model.Id);
            if (item == null)
            {
                return RedirectToAction("Index", "Admin");
            }

            item.AdminEditUser(model);
            _adminRepository.UpdateUser(item);
            return RedirectToAction("Index", "Admin");
        }

        public IActionResult Delete(Guid id)
        {
            _adminRepository.DeleteUser(id);
            return RedirectToAction("Index", "Admin");
        }

        [HttpGet]
        [Route("AdminProductEdit")]
        public IActionResult AdminProductEdit(int id, IFormFile imageFile)
        {
            Product item = _adminRepository.GetProductById(id);
            if (item == null)
            {
                return RedirectToAction("AdminProduct", "Admin");
            }

            return View(AdminServices.CreateProductEditVM(item));
        }

        [HttpPost]
        public IActionResult AdminProductEdit(Shopily.Domain.ViewModel.Products.EditVM model)
        {
            if (model.Image != null)
            {
                
                string fileName = _adminRepository.UploadImage(model.Image, _env);
                model.ImagePath = fileName; 
            }

            
            Product item = new Product(model);
            _adminRepository.UpdateProduct(item);

            return RedirectToAction("AdminProduct", "Admin");
        }

        public IActionResult AdminProductDelete(int id)
        {
            _adminRepository.DeleteProduct(id);
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

            string fileName =_adminRepository.UploadImage(model.ImagePath, _env);

            Product item = new Product(model, fileName);

            _adminRepository.AddProduct(item);
            return RedirectToAction("AdminProduct", "Admin");
        }



        [HttpGet]
        [Route("AdminStatus")]
        public IActionResult AdminStatus()
        {
            List<OrderStatus> ordersStatus = _context.Orders
                .Select(order => AdminServices.OrdersStatus(order))
                .ToList();

            if (!ordersStatus.Any())
            {
                return View(new List<OrderStatus>()); 
            }

            return View(ordersStatus);
        }

        [HttpPost]
        [Route("AdminStatus")] 
        public IActionResult AdminStatus(Guid Id, string Status)
        {
            var order = _context.Orders.FirstOrDefault(o => o.Id == Id);

            if (order == null)
            {
                return NotFound(); 
            }

            order.Status = Status;
            _context.SaveChanges(); 

            return RedirectToAction("AdminStatus");

        }

    }
}

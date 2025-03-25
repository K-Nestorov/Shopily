using Shopily.Data;
using Shopily.Domain.Entity;

namespace Shopily.Repositories
{
    public class AdminRepository
    {
        private readonly Context _context;

        public AdminRepository(Context context)
        {
            _context = context;
        }

        public List<User> GetAllUsers()
        {
            return _context.Users.ToList();
        }

        public User GetUserById(Guid id)
        {
            return _context.Users.FirstOrDefault(u => u.Id == id);
        }
        public void AddUser(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
        }

        public void UpdateUser(User user)
        {
            _context.Users.Update(user);
            _context.SaveChanges();
        }
        public void DeleteUser(Guid id)
        {
            User user = _context.Users.Find(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }
        }
        public List<Product> GetAllProducts()
        {
            return _context.Products.ToList();
        }

        public Product GetProductById(int id)
        {
            return _context.Products.FirstOrDefault(p => p.Id == id);
        }

        public void AddProduct(Product product)
        {
            _context.Products.Add(product);
            _context.SaveChanges();
        }

        public void UpdateStatus(Order order)
        {
            _context.Orders.Update(order);
            _context.SaveChanges();
        }
        public void UpdateProduct(Product product)
        {

            var existingProduct = _context.Products.FirstOrDefault(p => p.Id == product.Id);

            if (existingProduct != null)
            {
                existingProduct.ProductName = product.ProductName;
                existingProduct.Description = product.Description;
                existingProduct.Price = product.Price;
                existingProduct.Gender = product.Gender;
                existingProduct.Type = product.Type;

                if (!string.IsNullOrEmpty(product.ImagePath))
                {
                    existingProduct.ImagePath = product.ImagePath;
                }

                _context.SaveChanges();
            }
        }

        public void DeleteProduct(int id)
        {
            Product product = _context.Products.Find(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                _context.SaveChanges();
            }
        }
        public string UploadImage(IFormFile ImagePath, IWebHostEnvironment _env)
        {
            string fileName = null;
            if (ImagePath != null)
            {
                string uploadsFolder = Path.Combine(_env.WebRootPath, "Images");
                fileName = Guid.NewGuid().ToString() + "_" + ImagePath.FileName;
                string filePath = Path.Combine(uploadsFolder, fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                   ImagePath.CopyTo(fileStream);
                }
            }
            return fileName;
        }
    }
}

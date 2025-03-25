using Shopily.Domain.Entity;
using Shopily.Domain.ViewModel.Products;

namespace Shopily.Services
{
    public class ProductServices
    {
        public static SingleProductVM AddSigneProduct(Product product)
        {
            SingleProductVM model= new SingleProductVM
            {
              Id = product.Id,
                ProductName = product.ProductName,
                Description = product.Description,
                Price = product.Price,
                Gender = product.Gender,
                Image = product.ImagePath
            };
            return model;
        }
    }
}

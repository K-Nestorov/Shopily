using Shopily.Domain.Entity;
using Shopily.Domain.ViewModel.Admin;
using Shopily.Domain.ViewModel.Orders;

namespace Shopily.Services
{
    public class AdminServices
    {
        public static Domain.ViewModel.Products.EditVM CreateProductEditVM(Product item)
        {
            return new Domain.ViewModel.Products.EditVM
            {
                Id = item.Id,
                ProductName = item.ProductName,
                Price = item.Price,
                Description = item.Description,
                ImagePath = item.ImagePath,
                Gender = item.Gender,
                Type = item.Type,
             
            };
        }

        public static AdminEditVM CreateAdminEditVM(User item)
        {
            return new AdminEditVM
            {
                Id = item.Id,
                Username = item.Username,
                FirstName = item.FirstName,
                LastName = item.LastName,
                Email = item.Email,
                IsAdmin = item.IsAdmin,
            };
        }
        public static OrderStatus OrdersStatus(Order item)
        {
            return new OrderStatus
            {
                Id = item.Id,
                Status = item.Status,
            };
        }

    }
}

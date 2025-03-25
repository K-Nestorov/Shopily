using Shopily.Domain.Entity;
using Shopily.Domain.ViewModel.Cart;

namespace Shopily.Domain.ViewModel.Orders
{
    public class OrderVM : Order
    {
        public List<CartItemVM> Cart { get; set; }

        public OrderVM(Guid userId, List<CartItemVM> cart)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            Cart = cart;
            TotalCost = cart.Sum(x => x.Price * x.Quantity);
            ProductIds = string.Join(",", cart.Select(x => x.ProductId));
            ProductQuantity = string.Join(",", cart.Select(x => x.Quantity));
            OrderDate = DateTime.Now;
            Status = "Ordered";
        }
        public OrderVM()
        {
            Id = Guid.NewGuid();
            OrderDate = DateTime.Now;
        }
    }
}

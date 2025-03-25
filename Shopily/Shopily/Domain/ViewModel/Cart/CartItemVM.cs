namespace Shopily.Domain.ViewModel.Cart
{
    public class CartItemVM
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ImagePath { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; } 
        public Guid UserId { get; set; }

    }
}

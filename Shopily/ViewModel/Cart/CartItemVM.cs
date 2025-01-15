namespace Shopily.ViewModel.Cart
{
    public class CartItemVM
    {
        public int ProductId {  get; set; }
        public string ProductName { get; set; }
        public IFormFile ImagePath { get; set; }
        public double Price {  get; set; }
        public Guid UserId { get; set; }
    }
}

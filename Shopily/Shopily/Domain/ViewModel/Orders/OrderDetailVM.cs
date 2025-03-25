using Shopily.Domain.Entity;

namespace Shopily.Domain.ViewModel.Orders
{
    public class OrderDetailVM
    {
        public Guid OrderId { get; set; }
        public Guid UserId { get; set; }
        public string Status { get; set; }
        public DateTime OrderDate { get; set; }
        public double TotalCost { get; set; }
        public string OrderNotes { get; set; }
        public string Address { get; set; }

        public string UserFullName { get; set; }
        public string UserEmail { get; set; }

        public List<OrderItemVM> OrderItems { get; set; }

        public OrderDetailVM()
        {
            OrderItems = new List<OrderItemVM>();
        }
    }

    public class OrderItemVM
    {
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
        public string ImagePath { get; set; }
    }

}

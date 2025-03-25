using Shopily.Domain.ViewModel.Orders;
using Shopily.Domain.Entity.Base;

namespace Shopily.Domain.Entity
{
    public class Order : BaseEntity
    {

        public Guid UserId { get; set; }
        public string Country { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string OrderNotes { get; set; }
        public string Address { get; set; }
        public User User { get; set; }
        public double TotalCost { get; set; }
        public DateTime OrderDate { get; set; }
        public string ProductIds { get; set; }

        public string Status { get; set; }
        public string ProductQuantity { get; set; }

        public Order()
        {
            Id = Guid.NewGuid();

        }

        public Order(OrderVM item)
        {
            Id = item.Id;
            UserId = item.UserId;
            Country = item.Country;
            FirstName = item.FirstName;
            LastName = item.LastName;
            Email = item.Email;
            Phone = item.Phone;
            OrderNotes = item.OrderNotes;
            Address = item.Address;
            TotalCost = item.TotalCost;
            OrderDate = item.OrderDate;
            ProductIds = item.ProductIds;
            Status = item.Status;
            ProductQuantity = item.ProductQuantity;

        }
    }
}

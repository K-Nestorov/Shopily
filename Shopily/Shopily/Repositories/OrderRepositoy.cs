using Shopily.Data;
using Shopily.Domain.Entity;

namespace Shopily.Repositories
{
    public class OrderRepositoy
    {
        private readonly Context _context;

        public OrderRepositoy(Context context)
        {
            _context = context;
        }

        public void SaveOrder(Order order)
        {
            _context.Orders.Add(order);
            _context.SaveChanges();
        }
    }
}

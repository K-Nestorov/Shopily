using System.ComponentModel.DataAnnotations.Schema;
using Shopily.Domain.Entity.Base;

namespace Shopily.Domain.Entity
{
    public class Cart : BaseEntity
    {

        [ForeignKey("User")]
        public Guid UserId { get; set; }
        public User User { get; set; }
        [ForeignKey("Product")]
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int Quantity { get; set; }
    }
}

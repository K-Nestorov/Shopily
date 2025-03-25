using System.ComponentModel.DataAnnotations;

namespace Shopily.Domain.Entity.Base
{
    public abstract class BaseEntity
    {
        [Key]
        public Guid Id { get; set; }
    }
}

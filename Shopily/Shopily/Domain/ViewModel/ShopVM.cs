using Shopily.Domain.Entity;
using Shopily.Domain.ViewModel.Pages;

namespace Shopily.Domain.ViewModel
{
    public class ShopVM
    {
        public IEnumerable<Product> Products { get; set; }
        public PagerVM Pager { get; set; }
    }
}

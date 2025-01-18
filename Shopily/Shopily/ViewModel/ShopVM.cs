using Shopily.Entity;
using Shopily.ViewModel.Pages;

namespace Shopily.ViewModel
{
    public class ShopVM
    {
        public IEnumerable<Product> Products { get; set; }
        public PagerVM Pager { get; set; }
    }
}

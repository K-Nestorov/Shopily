using Shopily.Domain.ViewModel.Pages;
using static Shopily.Domain.ViewModel.Products.IndexVM;

namespace Shopily.Domain.ViewModel.Cart
{
    public class IndexVM
    {
        public List<CartItemVM> Items { get; set; } = new List<CartItemVM>();
        public FilterProductsVM FilterProductsVM { get; set; }

    }
}

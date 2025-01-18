using Shopily.ViewModel.Pages;
using static Shopily.ViewModel.Products.IndexVM;

namespace Shopily.ViewModel.Cart
{
    public class IndexVM
    {
        public List<CartItemVM> Items { get; set; }= new List<CartItemVM>();
        public FilterProductsVM FilterProductsVM { get; set; }

    }
}

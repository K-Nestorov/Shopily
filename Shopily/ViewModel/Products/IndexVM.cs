using Shopily.Entity;
<<<<<<< HEAD
using Shopily.ViewModel.Cart;
=======
>>>>>>> e752cef18d7408f29f6e3814efda787e4d92bb84
using Shopily.ViewModel.Pages;

namespace Shopily.ViewModel.Products
{
    public class IndexVM
    {
<<<<<<< HEAD
      
            public List<Product> Items { get; set; }
        public PagerVM Pager { get; set; }
        public List<CartItemVM> CartItems { get; set; }
        public List<Product> TotalItems { get; set; }
        public string ProductName { get; set; }
        public string Category { get; set; }
        public string ProductType { get; set; }
        public int MaleCount { get; set; }
        public int FemaleCount { get; set; }
        public int KidsCount { get; set; }
        public List<RecentlyViewedProduct> RecentlyViewedProducts { get; set; } = new List<RecentlyViewedProduct>();

=======
        public List<Product> Items { get; set; }
        public PagerVM Pager { get; set; }

        // Add these properties to hold category counts
        public string ProductName { get; set; }
        public string Category { get; set; }
        public string ProductType { get; set; }

        public int MaleCount { get; set; }
        public int FemaleCount { get; set; }
        public int KidsCount { get; set; }
>>>>>>> e752cef18d7408f29f6e3814efda787e4d92bb84
    }
}

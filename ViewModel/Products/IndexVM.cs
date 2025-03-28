﻿using Shopily.Entity;
using Shopily.ViewModel.Cart;
using Shopily.ViewModel.Pages;

namespace Shopily.ViewModel.Products
{
    public class IndexVM
    {
      
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

    }
}

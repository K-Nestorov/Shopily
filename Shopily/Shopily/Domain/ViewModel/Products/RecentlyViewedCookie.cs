using Azure.Core;
using System.Text.Json;

namespace Shopily.Domain.ViewModel.Products
{
    public class RecentlyViewedCookie
    {
        public int ProductId { get; set; }
        public string ImagePath { get; set; }
        public string Description { get; set; }
        public string ProductName { get; set; }
        public double Price { get; set; }

    }



}

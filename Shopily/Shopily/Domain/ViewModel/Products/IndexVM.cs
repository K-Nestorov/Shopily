using Shopily.Domain.Entity;
using Shopily.Domain.ViewModel.Pages;

namespace Shopily.Domain.ViewModel.Products
{
    public class IndexVM
    {
        public List<Product> Items { get; set; }
        public PagerVM Pager { get; set; }

        public string ProductName { get; set; }
        public string Category { get; set; }
        public string ProductType { get; set; }

        public int MaleCount { get; set; }
        public int FemaleCount { get; set; }
        public int KidsCount { get; set; }
    }
}

using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Shopily.Domain.ViewModel.Admin;
using Shopily.Domain.ViewModel.Products;
using System.ComponentModel.DataAnnotations;

namespace Shopily.Domain.Entity
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public string Gender { get; set; }
        public string Type { get; set; }
        public int WareHouseQuantity { get; set; }
        public string ImagePath { get; set; }
        public DateTime TimeOfAdd { get; set; }
        public Product()
        {

        }
        public Product(EditVM model)
        {
            Id = model.Id;
            ProductName = model.ProductName;
            Description = model.Description;
            Price = model.Price;
            Gender = model.Gender;
            Type = model.Type;
            ImagePath = model.ImagePath;

        }
        public Product(CreateVM model, string fileName)
        {
            Description = model.Description;
            Price = model.Price;
            ProductName = model.ProductName;
            Gender = model.Gender;
            WareHouseQuantity = model.WareHouseQuantity;
            Type = model.Type;
            TimeOfAdd = DateTime.Now;
            ImagePath = fileName;
        }
    }

}

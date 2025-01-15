using Shopily.ViewModel.Admin;
using Shopily.ViewModel.Pages;
using Shopily.ViewModel.Products;
using System.ComponentModel.DataAnnotations;

namespace Shopily.Entity
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
            this.Id = model.Id;
            this.ProductName = model.ProductName;
            this.Description = model.Description;
            this.Price = model.Price;
            this.Gender = model.Gender;
            this.Type = model.Type;
            this.ImagePath = model.ImagePath;
           
        }
        public void ProductCreate(CreateVM model)
        {
            this.ProductName = model.ProductName;
            this.Price = model.Price;
            this.Gender = model.Gender;
            this.Type = model.Type;
            this.WareHouseQuantity = model.WareHouseQuantity;
          
            this.TimeOfAdd= DateTime.Now;
        }
    }

}

namespace Shopily.ViewModel.Admin
{
    public class CreateVM
    {
        public string ProductName { get; set; }      
        public string Description { get; set; }
        public double Price { get; set; }
        public string Gender { get; set; }
        public string Type { get; set; }
        public int WareHouseQuantity { get; set; }
        public IFormFile ImagePath { get; set; }
        public DateTime DateTimeAdd=DateTime.Now;
    }
}

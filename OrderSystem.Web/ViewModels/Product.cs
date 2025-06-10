namespace OrderSystem.Web.ViewModels
{
    public class Product
    {
        public int Id { get; set; }
        
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public int StockQuantity { get; set; }

        public decimal Price { get; set; }
    }
}

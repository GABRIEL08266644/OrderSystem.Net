namespace OrderSystem.Domain.Models
{
    public class Order
    {
        public int Id { get; set; }
        
        public int ClientId { get; set; }

        public Client Client { get; set; }

        public DateTime OrderDate { get; set; }

        public int Quantity { get; set; }

        public decimal TotalAmount { get; set; }

        public string Status { get; set; }

        public List<OrderItem> Items { get; set; } = new();
    }
}

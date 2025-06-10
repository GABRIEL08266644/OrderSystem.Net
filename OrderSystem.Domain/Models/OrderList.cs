using OrderSystem.Domain.Models;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace OrderSystem.Domain.Models;
public class OrderList
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Select a customer.")]
    public int ClientId { get; set; }

    public virtual Client? Client { get; set; }

    public DateTime OrderDate { get; set; }

    public decimal TotalAmount { get; set; }
    
    public int Quantity { get; set; }

    public string Status { get; set; } = string.Empty;

    public List<int> ProductIds { get; set; } = new();
    public List<OrderItem> Items { get; set; } = new();

    public List<Product>? Products { get; set; }
}


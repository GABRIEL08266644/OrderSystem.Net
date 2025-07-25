﻿namespace OrderSystem.Web.ViewModels
{
    public class OrderItem
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; } = string.Empty;

        public decimal UnitPrice { get; set; }

        public int Quantity { get; set; }
        
        public decimal Total => UnitPrice * Quantity;
    }
}

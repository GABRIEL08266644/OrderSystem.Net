using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OrderSystem.Web.ViewModels
{
    public class OrderViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Cliente")]
        public int ClientId { get; set; }

        public string ClientName { get; set; } = string.Empty;

        [Display(Name = "Data do Pedido")]
        public DateTime OrderDate { get; set; } = DateTime.Now;

        public string Status { get; set; } = "New";

        public List<OrderItem> Items { get; set; } = new();

        public decimal Total => Items.Sum(i => i.Total);
    }
}

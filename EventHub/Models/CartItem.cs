using System.ComponentModel.DataAnnotations;

namespace EventHub.Models
{
    public class CartItem
    {
        public int EventId { get; set; }
        public string EventName { get; set; } = string.Empty;
        public string TicketType { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal TotalPrice => Price * Quantity;
    }
} 
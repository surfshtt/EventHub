using System.ComponentModel.DataAnnotations;

namespace EventHub.Models
{
    public class UserTicket
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public int EventId { get; set; }
        public Event Event { get; set; }

        [Required]
        public DateTime PurchaseDate { get; set; }

        [Required]
        public string Status { get; set; } = "Active"; // Active, Used, Cancelled

        [Required]
        public int Quantity { get; set; }

        [Required]
        public decimal TotalPrice { get; set; }
    }
} 
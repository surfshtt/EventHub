using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventHub.Models
{
    public class Event
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        public string Description { get; set; } = string.Empty;
        
        [Required]
        public string Date { get; set; } = string.Empty;
        
        [Required]
        public string Country { get; set; } = string.Empty;
        
        [Required]
        public string Place { get; set; } = string.Empty;
        
        [Required]
        public int NeedableAge { get; set; }
        
        [Required]
        public decimal Price { get; set; }

        [Required]
        [StringLength(50)]
        public string EventType { get; set; } = string.Empty;

        [Required]
        public byte[] Picture { get; set; } = Array.Empty<byte>();

        [Required]
        public byte[] Poster { get; set; } = Array.Empty<byte>();
    }
}

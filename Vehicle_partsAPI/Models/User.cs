using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace vehicle_parts.Models
{
    public class User
    {
        [Key]
        public int UserID { get; set; }

        [Required]
        public int RoleID { get; set; }

        [Required]
        [MaxLength(100)]
        public required string FullName { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        public required string Phone { get; set; }

        [Required]
        public required string PasswordHash { get; set; }

        public string? Address { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public string? Status { get; set; } // e.g., Active, Inactive

        // Navigation properties
        [ForeignKey("RoleID")]
        public Role? Role { get; set; }
        public ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
        public ICollection<SalesInvoice> SalesInvoices { get; set; } = new List<SalesInvoice>();
    }
}

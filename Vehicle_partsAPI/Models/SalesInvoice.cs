using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace vehicle_parts.Models
{
    public class SalesInvoice
    {
        [Key]
        public int SalesInvoiceID { get; set; }

        [Required]
        public int UserID { get; set; }

        public DateTime SalesDate { get; set; } = DateTime.UtcNow;

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal DiscountAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal FinalAmount { get; set; }

        public required string PaymentStatus { get; set; } // Pending, Partially Paid, Paid

        public DateTime? DueDate { get; set; }

        // Navigation properties
        [ForeignKey("UserID")]
        public User? User { get; set; }
        public ICollection<SalesInvoiceDetail> Details { get; set; } = new List<SalesInvoiceDetail>();
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}

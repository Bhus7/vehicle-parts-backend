using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace vehicle_parts.Models
{
    public class SalesInvoice
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CustomerId { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal PaidAmount { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "Pending"; // e.g., Pending, Paid, PartiallyPaid

        // Navigation property
        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; }

        [NotMapped]
        public bool IsOverdue => (TotalAmount > PaidAmount) && (DateTime.UtcNow > CreatedAt.AddMonths(1));
        
        [NotMapped]
        public decimal CreditAmount => TotalAmount - PaidAmount;
    }
}

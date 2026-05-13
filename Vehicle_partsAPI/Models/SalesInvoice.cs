using System;
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

        [ForeignKey("UserID")]
        public User? Customer { get; set; }

        [Required]
        public DateTime SalesDate { get; set; } = DateTime.UtcNow;

        [Required]
        public decimal TotalAmount { get; set; }

        public decimal DiscountAmount { get; set; }

        [Required]
        public decimal FinalAmount { get; set; }

        public string PaymentStatus { get; set; } = "Pending";

        public DateTime? DueDate { get; set; }

        public ICollection<SalesInvoiceDetail>? Details { get; set; }
    }
}

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace vehicle_parts.Models
{
    public class Payment
    {
        [Key]
        public int PaymentID { get; set; }

        [Required]
        public int SalesInvoiceID { get; set; }

        [ForeignKey("SalesInvoiceID")]
        public SalesInvoice? SalesInvoice { get; set; }

        [Required]
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal AmountPaid { get; set; }

        public string? PaymentMethod { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal RemainingBalance { get; set; }
    }
}

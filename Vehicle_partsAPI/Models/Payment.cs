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
        public decimal AmountPaid { get; set; }

        public string PaymentMethod { get; set; }

        public decimal RemainingBalance { get; set; }
    }
}

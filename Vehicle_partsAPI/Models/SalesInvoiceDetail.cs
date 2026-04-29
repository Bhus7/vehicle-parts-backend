using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace vehicle_parts.Models
{
    public class SalesInvoiceDetail
    {
        [Key]
        public int SalesDetailID { get; set; }

        [Required]
        public int SalesInvoiceID { get; set; }

        [Required]
        public int PartID { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Subtotal { get; set; }

        // Navigation properties
        [ForeignKey("SalesInvoiceID")]
        public SalesInvoice? SalesInvoice { get; set; }
    }
}

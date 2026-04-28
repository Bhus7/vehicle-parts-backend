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

        [ForeignKey("SalesInvoiceID")]
        public SalesInvoice? SalesInvoice { get; set; }

        [Required]
        public int PartID { get; set; }

        [ForeignKey("PartID")]
        public Part? Part { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public decimal UnitPrice { get; set; }

        [Required]
        public decimal Subtotal { get; set; }
    }
}

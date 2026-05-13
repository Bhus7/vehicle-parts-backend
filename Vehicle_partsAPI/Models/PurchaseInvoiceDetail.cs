using System.ComponentModel.DataAnnotations;

namespace Vehicle_partsAPI.Models
{
    public class PurchaseInvoiceDetail
    {
        [Key]
        public int PurchaseDetailID { get; set; }

        public int PurchaseInvoiceID { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public PurchaseInvoice? PurchaseInvoice { get; set; }

        public int PartID { get; set; }
        public Part? Part { get; set; }

        public int Quantity { get; set; }
        public decimal UnitCost { get; set; }
        public decimal Subtotal { get; set; }
    }
}

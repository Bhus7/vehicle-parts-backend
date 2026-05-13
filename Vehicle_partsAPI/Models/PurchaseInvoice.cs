using System.ComponentModel.DataAnnotations;

namespace Vehicle_partsAPI.Models
{
    public class PurchaseInvoice
    {
        [Key]
        public int PurchaseInvoiceID { get; set; }
        
        public int VendorID { get; set; }
        public Vendor? Vendor { get; set; }

        public int UserID { get; set; }

        public DateTime PurchaseDate { get; set; }
        public decimal TotalAmount { get; set; }

        public ICollection<PurchaseInvoiceDetail> Details { get; set; } = new List<PurchaseInvoiceDetail>();
    }
}

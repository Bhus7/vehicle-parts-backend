using System.ComponentModel.DataAnnotations;

namespace Vehicle_partsAPI.DTOs
{
    public class PurchaseInvoiceCreateDto
    {
        [Required]
        public int VendorID { get; set; }

        [Required]
        public int UserID { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "A purchase must contain at least one item.")]
        public List<PurchaseInvoiceDetailCreateDto> Items { get; set; } = new List<PurchaseInvoiceDetailCreateDto>();
    }

    public class PurchaseInvoiceDetailCreateDto
    {
        [Required]
        public int PartID { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Unit cost must be greater than zero.")]
        public decimal UnitCost { get; set; }
    }
}

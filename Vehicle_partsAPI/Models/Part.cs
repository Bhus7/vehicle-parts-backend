using System.ComponentModel.DataAnnotations;

namespace Vehicle_partsAPI.Models
{
    public class Part
    {
        [Key]
        public int PartID { get; set; }
        public string PartName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int StockQuantity { get; set; }
        public int ReorderLevel { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace vehicle_parts.Models
{
    public class Part
    {
        [Key]
        public int PartID { get; set; }

        [Required]
        public string PartName { get; set; }

        public string Category { get; set; }

        public string Description { get; set; }

        [Required]
        public decimal UnitPrice { get; set; }

        [Required]
        public int StockQuantity { get; set; }

        public int ReorderLevel { get; set; } = 10;
    }
}

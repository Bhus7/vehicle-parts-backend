using System.ComponentModel.DataAnnotations;

namespace vehicle_parts.Models
{
    public class PartRequest
    {
        public int Id { get; set; }

        [Required]
        public string PartName { get; set; }

        public string? Description { get; set; }

        public string Status { get; set; } = "Pending";

        public int CustomerId { get; set; }
    }
}

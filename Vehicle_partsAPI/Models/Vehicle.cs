using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace vehicle_parts.Models
{
    public class Vehicle
    {
        [Key]
        public int VehicleID { get; set; }

        [Required]
        public int UserID { get; set; }

        [Required]
        [MaxLength(20)]
        public required string VehicleNumber { get; set; }

        public string? Brand { get; set; }

        public string? Model { get; set; }

        public int Year { get; set; }

        public string? VehicleType { get; set; }

        public string? ConditionNotes { get; set; }

        // Navigation properties
        [ForeignKey("UserID")]
        public User? User { get; set; }
    }
}

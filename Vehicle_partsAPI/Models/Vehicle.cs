using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace vehicle_parts.Models
{
    public class Vehicle
    {
        [Key]
        public int VehicleID { get; set; }

        [Required]
        public int UserID { get; set; }

        [JsonIgnore]
        [ForeignKey("UserID")]
        public User? User { get; set; }

        [Required]
        [MaxLength(20)]
        public string VehicleNumber { get; set; } = string.Empty;

        public string? Brand { get; set; }

        public string? Model { get; set; }

        public int Year { get; set; }

        public string? VehicleType { get; set; }

        public string? ConditionNotes { get; set; }
    }
}

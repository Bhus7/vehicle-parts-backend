using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace vehicle_parts.Models
{
    public class Appointment
    {
        [Key]
        public int AppointmentID { get; set; }

        [Required]
        public int UserID { get; set; }
        [JsonIgnore]
        public User? User { get; set; }

        [Required]
        public int VehicleID { get; set; }

        [JsonIgnore]
        public Vehicle? Vehicle { get; set; }

        [Required]
        public DateTime AppointmentDate { get; set; }

        public string ServiceType { get; set; }

        public string Status { get; set; } = "Scheduled";

        public string Notes { get; set; }
    }
}
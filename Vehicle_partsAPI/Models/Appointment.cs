using System;

namespace vehicle_parts.Models
{
    public class Appointment
    {
        public int Id { get; set; }

        public string VehicleNumber { get; set; }

        public DateTime AppointmentDate { get; set; }

        public string Description { get; set; }

        public string Status { get; set; } = "Pending";

        public int CustomerId { get; set; }
    }
}
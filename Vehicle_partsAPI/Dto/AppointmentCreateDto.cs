using System;

namespace vehicle_parts.Dto
{
    public class AppointmentCreateDto
    {
        public int UserID { get; set; }
        public int VehicleID { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string ServiceType { get; set; }
        public string Status { get; set; }
        public string Notes { get; set; }
    }
}

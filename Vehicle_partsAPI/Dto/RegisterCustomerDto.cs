using System;

namespace vehicle_parts.Dto
{
    public class RegisterCustomerDto
    {
        // User (Customer) Details
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; } // Will be hashed or stored as PasswordHash
        public string Address { get; set; }

        // Vehicle Details
        public string VehicleNumber { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public string VehicleType { get; set; }
        public string ConditionNotes { get; set; }
    }
}

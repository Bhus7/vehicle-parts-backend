using System;

namespace vehicle_parts.Dto.Responses
{
    public class StaffResponse
    {
        public int UserID { get; set; }
        public int RoleID { get; set; }
        public string RoleName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Status { get; set; }
    }
}

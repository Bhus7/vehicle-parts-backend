using System.ComponentModel.DataAnnotations;

namespace vehicle_parts.Dto.Requests
{
    public class UpdateStaffRequest
    {
        [StringLength(100, ErrorMessage = "Full Name cannot exceed 100 characters.")]
        public string FullName { get; set; }

        [Phone(ErrorMessage = "Invalid Phone format.")]
        [StringLength(20, ErrorMessage = "Phone cannot exceed 20 characters.")]
        public string Phone { get; set; }

        public string Address { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace Vehicle_partsAPI.Models
{
    public class Vendor
    {
        [Key]
        public int VendorID { get; set; }
        public string VendorName { get; set; } = string.Empty;
        public string ContactPerson { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
    }
}

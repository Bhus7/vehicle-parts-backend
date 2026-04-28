using System.ComponentModel.DataAnnotations;

namespace vehicle_parts.Models
{
    public class Role
    {
        [Key]
        public int RoleID { get; set; }
        [Required]
        public string RoleName { get; set; }
    }
}

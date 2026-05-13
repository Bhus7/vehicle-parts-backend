using System.ComponentModel.DataAnnotations;

namespace vehicle_parts.Models
{
    public class Role
    {
        [Key]
        public int RoleID { get; set; }
        [Required]
        public required string RoleName { get; set; }

        // Navigation properties
        public ICollection<User> Users { get; set; } = new List<User>();

        [Required]
        [MaxLength(50)]
        public string RoleName { get; set; }
    }
}

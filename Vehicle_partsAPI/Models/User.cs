using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace vehicle_parts.Models
{
    public class User
    {
        [Key]
        public int UserID { get; set; }

        [Required]
        public int RoleID { get; set; }

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; }

        [Required]
        [MaxLength(150)]
        public string Email { get; set; }

        [MaxLength(20)]
        public string Phone { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        public string Address { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "Active";

        // Navigation property
        [ForeignKey("RoleID")]
        public Role Role { get; set; }
    }
}

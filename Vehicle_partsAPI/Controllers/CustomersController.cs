using Microsoft.AspNetCore.Mvc;
using vehicle_parts.Data;
using vehicle_parts.Models;
using vehicle_parts.Dto;
using System.Linq;

namespace vehicle_parts.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Tags("3. User Authentication & Profile")]
    public class CustomersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CustomersController(AppDbContext context)
        {
            _context = context;
        }

        // REGISTER
        [HttpPost("register")]
        public IActionResult Register(UserRegisterDto dto)
        {
            // Basic null check
            if (dto == null)
                return BadRequest("Invalid data");

            // Required field validation
            if (string.IsNullOrWhiteSpace(dto.FullName) ||
                string.IsNullOrWhiteSpace(dto.Email) ||
                string.IsNullOrWhiteSpace(dto.Password) ||
                string.IsNullOrWhiteSpace(dto.Phone))
            {
                return BadRequest("All fields are required");
            }

            // Email format check
            if (!dto.Email.Contains("@") || !dto.Email.Contains("."))
            {
                return BadRequest("Invalid email format");
            }

            // Password length check
            if (dto.Password.Length < 4)
            {
                return BadRequest("Password must be at least 4 characters");
            }

            // Phone number length check (basic)
            if (dto.Phone.Length < 10)
            {
                return BadRequest("Invalid phone number");
            }

            // Check duplicate email (case-insensitive)
            var exists = _context.Users
                .Any(u => u.Email.ToLower() == dto.Email.ToLower());

            if (exists)
                return BadRequest("Email already exists");

            // Create User from DTO
            var user = new User
            {
                FullName = dto.FullName,
                Email = dto.Email,
                Phone = dto.Phone,
                PasswordHash = dto.Password,
                Address = dto.Address,
                RoleID = 3, // Default role is Customer
                CreatedDate = System.DateTime.UtcNow,
                Status = "Active"
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            // Clean response
            return Ok(new
            {
                user.UserID,
                user.FullName,
                user.Email,
                user.Phone,
                user.RoleID
            });
        }

        // LOGIN
        [HttpPost("login")]
        public IActionResult Login(LoginDto login)
        {
            // Admin Login (Hardcoded for initial testing)
            if (login.Email == "admin@gmail.com" && login.Password == "admin123")
            {
                return Ok(new
                {
                    UserID = 0,
                    FullName = "Admin",
                    Email = "admin@gmail.com",
                    Phone = "",
                    RoleID = 1 // Admin Role
                });
            }

            // Normal User Login
            var user = _context.Users
                .FirstOrDefault(u => u.Email == login.Email && u.PasswordHash == login.Password);

            if (user == null)
                return Unauthorized("Invalid credentials");

            return Ok(new
            {
                user.UserID,
                user.FullName,
                user.Email,
                user.Phone,
                user.RoleID
            });
        }

        [HttpGet]
        public IActionResult GetAllCustomers()
        {
            var customers = _context.Users
                .Where(u => u.RoleID == 3) // Only customers
                .Select(u => new {
                    u.UserID,
                    u.FullName,
                    u.Email,
                    u.Phone
                }).ToList();
            
            return Ok(customers);
        }
    }
}

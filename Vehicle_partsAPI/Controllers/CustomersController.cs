using Microsoft.AspNetCore.Mvc;
using vehicle_parts.Data;
using vehicle_parts.Models;
using System.Linq;



namespace vehicle_parts.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CustomersController(AppDbContext context)
        {
            _context = context;
        }

        // REGISTER
        [HttpPost("register")]
        public IActionResult Register(Customer customer)
        {
            // Basic null check
            if (customer == null)
                return BadRequest("Invalid data");

            // Required field validation
            if (string.IsNullOrWhiteSpace(customer.Name) ||
                string.IsNullOrWhiteSpace(customer.Email) ||
                string.IsNullOrWhiteSpace(customer.Password) ||
                string.IsNullOrWhiteSpace(customer.Phone))
            {
                return BadRequest("All fields are required");
            }

            // Email format check
            if (!customer.Email.Contains("@") || !customer.Email.Contains("."))
            {
                return BadRequest("Invalid email format");
            }

            // Password length check
            if (customer.Password.Length < 4)
            {
                return BadRequest("Password must be at least 4 characters");
            }

            // Phone number length check (basic)
            if (customer.Phone.Length < 10)
            {
                return BadRequest("Invalid phone number");
            }

            // Check duplicate email (case-insensitive)
            var exists = _context.Customers
                .Any(c => c.Email.ToLower() == customer.Email.ToLower());

            if (exists)
                return BadRequest("Email already exists");

            // Force role
            customer.Role = "Customer";

            _context.Customers.Add(customer);
            _context.SaveChanges();

            // Clean response
            return Ok(new
            {
                customer.Id,
                customer.Name,
                customer.Email,
                customer.Phone,
                customer.Role
            });
        }

        // LOGIN
        [HttpPost("login")]
        public IActionResult Login(LoginDto login)
        {
            // Admin Login
            if (login.Email == "admin@gmail.com" && login.Password == "admin123")
            {
                return Ok(new
                {
                    Id = 0,
                    Name = "Admin",
                    Email = "admin@gmail.com",
                    Phone = "",
                    Role = "Admin"
                });
            }

            // 🔹 Normal Customer Login
            var user = _context.Customers
                .FirstOrDefault(c => c.Email == login.Email && c.Password == login.Password);

            if (user == null)
                return Unauthorized("Invalid credentials");

            return Ok(new
            {
                user.Id,
                user.Name,
                user.Email,
                user.Phone,
                user.Role
            });
        }

        [HttpGet]
        public IActionResult GetAllCustomers()
        {
            var customers = _context.Customers.ToList();
            return Ok(_context.Customers.Select(c => new {
                c.Id,
                c.Name,
                c.Email,
                c.Phone,
                c.Role
            }));
        }
    }
}

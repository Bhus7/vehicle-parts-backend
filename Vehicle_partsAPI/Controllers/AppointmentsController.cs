using Microsoft.AspNetCore.Mvc;
using vehicle_parts.Data;
using vehicle_parts.Models;
using System.Linq;

namespace vehicle_parts.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppointmentsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AppointmentsController(AppDbContext context)
        {
            _context = context;
        }

        // Create appointment
        [HttpPost]
        public IActionResult Create(Appointment appointment)
        {
            if (string.IsNullOrEmpty(appointment.VehicleNumber) ||
                string.IsNullOrEmpty(appointment.Description) ||
                appointment.CustomerId == 0)
            {
                return BadRequest("All fields are required");
            }

            if (appointment.AppointmentDate == default)
            {
                return BadRequest("Invalid date");
            }

            appointment.Status = "Pending";
            appointment.Id = 0; // Ensure the ID is handled by the database

            _context.Appointments.Add(appointment);
            _context.SaveChanges();

            return Ok(appointment);
        }

        // Get by customer
        [HttpGet("{customerId}")]
        public IActionResult GetByCustomer(int customerId)
        {
            var appointments = _context.Appointments
                .Where(a => a.CustomerId == customerId)
                .ToList();

            return Ok(appointments);
        }
    }
}

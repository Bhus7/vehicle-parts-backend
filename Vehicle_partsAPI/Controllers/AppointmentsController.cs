using Microsoft.AspNetCore.Mvc;
using vehicle_parts.Data;
using vehicle_parts.Models;
using vehicle_parts.Dto;
using System.Linq;

namespace vehicle_parts.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Tags("2. Appointments")]
    public class AppointmentsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AppointmentsController(AppDbContext context)
        {
            _context = context;
        }

        // Create appointment
        [HttpPost]
        public IActionResult Create(AppointmentCreateDto dto)
        {
            if (dto.VehicleID == 0 ||
                string.IsNullOrEmpty(dto.ServiceType) ||
                dto.UserID == 0)
            {
                return BadRequest("All fields are required");
            }

            if (dto.AppointmentDate == default)
            {
                return BadRequest("Invalid date");
            }

            var appointment = new Appointment
            {
                UserID = dto.UserID,
                VehicleID = dto.VehicleID,
                AppointmentDate = dto.AppointmentDate,
                ServiceType = dto.ServiceType,
                Notes = dto.Notes,
                Status = string.IsNullOrEmpty(dto.Status) ? "Pending" : dto.Status
            };
            appointment.AppointmentID = 0; // Ensure the ID is handled by the database

            _context.Appointments.Add(appointment);
            _context.SaveChanges();

            return Ok(appointment);
        }

        // Get by customer
        [HttpGet("{userId}")]
        public IActionResult GetByCustomer(int userId)
        {
            var appointments = _context.Appointments
                .Where(a => a.UserID == userId)
                .ToList();

            return Ok(appointments);
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using vehicle_parts.Dto.Response;
using vehicle_parts.Services;

namespace vehicle_parts.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    // [Authorize(Roles = "Staff")] // Basic authorization rule
    public class StaffController : ControllerBase
    {
        private readonly IStaffService _staffService;

        public StaffController(IStaffService staffService)
        {
            _staffService = staffService;
        }

        [HttpGet("search-customers")]
        public async Task<ActionResult<IEnumerable<CustomerSearchResponse>>> SearchCustomers([FromQuery] string query)
        {
            var results = await _staffService.SearchCustomersAsync(query);
            return Ok(results);
        }

        [HttpGet("reports/regular-customers")]
        public async Task<ActionResult<IEnumerable<CustomerReportResponse>>> GetRegularCustomers()
        {
            var results = await _staffService.GetRegularCustomersAsync();
            return Ok(results);
        }

        [HttpGet("reports/high-spenders")]
        public async Task<ActionResult<IEnumerable<CustomerReportResponse>>> GetHighSpenders()
        {
            var results = await _staffService.GetHighSpendersAsync();
            return Ok(results);
        }

        [HttpGet("reports/pending-credits")]
        public async Task<ActionResult<IEnumerable<CustomerReportResponse>>> GetPendingCredits()
        {
            var results = await _staffService.GetPendingCreditsAsync();
            return Ok(results);
        }
    }
}

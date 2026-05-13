using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using vehicle_parts.Dto.Requests;
using vehicle_parts.Services;

namespace vehicle_parts.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    // [Authorize(Roles = "ADMIN")] // Uncomment when Authentication is fully configured
    public class StaffController : ControllerBase
    {
        private readonly IStaffService _staffService;

        public StaffController(IStaffService staffService)
        {
            _staffService = staffService;
        }

        // POST /api/staff
        [HttpPost]
        public async Task<IActionResult> CreateStaff([FromBody] CreateStaffRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _staffService.CreateStaffAsync(request);
            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        // GET /api/staff
        [HttpGet]
        public async Task<IActionResult> GetAllStaff()
        {
            var result = await _staffService.GetAllStaffAsync();
            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        // GET /api/staff/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetStaffById(int id)
        {
            var result = await _staffService.GetStaffByIdAsync(id);
            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }

        // PUT /api/staff/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStaff(int id, [FromBody] UpdateStaffRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _staffService.UpdateStaffAsync(id, request);
            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }

        // PATCH /api/staff/{id}/status
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> ChangeStaffStatus(int id, [FromBody] ChangeStatusRequest request)
        {
            var result = await _staffService.ChangeStaffStatusAsync(id, request.Status);
            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }

        // PATCH /api/staff/{id}/role
        [HttpPatch("{id}/role")]
        public async Task<IActionResult> ChangeStaffRole(int id, [FromBody] ChangeRoleRequest request)
        {
            var result = await _staffService.ChangeStaffRoleAsync(id, request.RoleName);
            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }

        // DELETE /api/staff/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStaff(int id)
        {
            var result = await _staffService.DeleteStaffAsync(id);
            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }
    }

    public class ChangeStatusRequest
    {
        public string Status { get; set; }
    }

    public class ChangeRoleRequest
    {
        public string RoleName { get; set; }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vehicle_partsAPI.Data;
using Vehicle_partsAPI.Models;

namespace Vehicle_partsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VendorsController(AppDbContext context) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Vendor>>> GetVendors()
        {
            return await context.Vendors.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Vendor>> GetVendor(int id)
        {
            var vendor = await context.Vendors.FindAsync(id);
            if (vendor == null) return NotFound();
            return vendor;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Vendor>> PostVendor(Vehicle_partsAPI.DTOs.VendorCreateDto dto)
        {
            var vendor = new Vendor
            {
                VendorName = dto.VendorName,
                ContactPerson = dto.ContactPerson,
                Phone = dto.Phone,
                Email = dto.Email,
                Address = dto.Address
            };

            context.Vendors.Add(vendor);
            await context.SaveChangesAsync();
            return Ok(vendor);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutVendor(int id, Vendor vendor)
        {
            if (id != vendor.VendorID) return BadRequest();
            context.Entry(vendor).State = EntityState.Modified;
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVendor(int id)
        {
            var vendor = await context.Vendors.FindAsync(id);
            if (vendor == null) return NotFound();
            context.Vendors.Remove(vendor);
            await context.SaveChangesAsync();
            return NoContent();
        }
    }
}

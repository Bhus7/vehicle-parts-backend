using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vehicle_partsAPI.Data;
using Vehicle_partsAPI.Models;

namespace Vehicle_partsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PartsController(AppDbContext context) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Part>>> GetParts()
        {
            return await context.Parts.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Part>> GetPart(int id)
        {
            var part = await context.Parts.FindAsync(id);
            if (part == null) return NotFound();
            return part;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Part>> PostPart(Vehicle_partsAPI.DTOs.PartCreateDto dto)
        {
            var part = new Part
            {
                PartName = dto.PartName,
                Category = dto.Category,
                Description = dto.Description,
                UnitPrice = dto.UnitPrice,
                StockQuantity = dto.StockQuantity,
                ReorderLevel = dto.ReorderLevel
            };

            context.Parts.Add(part);
            await context.SaveChangesAsync();
            return Ok(part);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutPart(int id, Part part)
        {
            if (id != part.PartID) return BadRequest();
            context.Entry(part).State = EntityState.Modified;
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePart(int id)
        {
            var part = await context.Parts.FindAsync(id);
            if (part == null) return NotFound();
            context.Parts.Remove(part);
            await context.SaveChangesAsync();
            return NoContent();
        }
    }
}

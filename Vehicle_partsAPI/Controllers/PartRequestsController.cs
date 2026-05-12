using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using vehicle_parts.Data;
using vehicle_parts.Models;

[Route("api/[controller]")]
[ApiController]
public class PartRequestsController : ControllerBase
{
    private readonly AppDbContext _context;

    public PartRequestsController(AppDbContext context)
    {
        _context = context;
    }

    //  POST: api/PartRequests
    [HttpPost]
    public async Task<IActionResult> CreateRequest(PartRequest request)
    {
        request.Status = "Pending";

        _context.PartRequests.Add(request);
        await _context.SaveChangesAsync();

        return Ok(request);
    }

    // GET: api/PartRequests/{customerId}
    [HttpGet("{customerId}")]
    public async Task<IActionResult> GetRequests(int customerId)
    {
        var requests = await _context.PartRequests
            .Where(r => r.CustomerId == customerId)
            .ToListAsync();

        return Ok(requests);
    }

    // Delete request
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRequest(int id)
    {
        var request = await _context.PartRequests.FindAsync(id);

        if (request == null)
            return NotFound();

        _context.PartRequests.Remove(request);
        await _context.SaveChangesAsync();

        return Ok();
    }
}

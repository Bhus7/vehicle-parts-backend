using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vehicle_partsAPI.Data;
using Vehicle_partsAPI.DTOs;
using Vehicle_partsAPI.Models;
using Vehicle_partsAPI.Services;

namespace Vehicle_partsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchaseInvoicesController(IPurchaseService purchaseService, AppDbContext context) : ControllerBase
    {
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<PurchaseInvoice>> CreatePurchase(PurchaseInvoiceCreateDto dto)
        {
            try
            {
                var invoice = await purchaseService.CreatePurchaseAsync(dto);
                return Ok(invoice);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PurchaseInvoice>> GetPurchaseInvoice(int id)
        {
            var invoice = await context.PurchaseInvoices
                .Include(i => i.Details)
                .ThenInclude(d => d.Part)
                .Include(i => i.Vendor)
                .FirstOrDefaultAsync(i => i.PurchaseInvoiceID == id);

            if (invoice == null) return NotFound();
            return invoice;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PurchaseInvoice>>> GetPurchaseInvoices()
        {
            return await context.PurchaseInvoices
                .Include(i => i.Vendor)
                .OrderByDescending(i => i.PurchaseDate)
                .ToListAsync();
        }
    }
}

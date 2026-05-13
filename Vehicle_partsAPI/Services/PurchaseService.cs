using Vehicle_partsAPI.Data;
using Vehicle_partsAPI.DTOs;
using Vehicle_partsAPI.Models;

namespace Vehicle_partsAPI.Services
{
    public class PurchaseService(AppDbContext context) : IPurchaseService
    {
        public async Task<PurchaseInvoice> CreatePurchaseAsync(PurchaseInvoiceCreateDto dto)
        {
            await using var transaction = await context.Database.BeginTransactionAsync();

            try
            {
                var invoice = new PurchaseInvoice
                {
                    VendorID = dto.VendorID,
                    UserID = dto.UserID,
                    PurchaseDate = DateTime.UtcNow,
                    TotalAmount = 0
                };

                foreach (var itemDto in dto.Items)
                {
                    var part = await context.Parts.FindAsync(itemDto.PartID);
                    if (part == null)
                    {
                        throw new Exception($"Part with ID {itemDto.PartID} not found.");
                    }

                    // Update stock level (Feature 4)
                    part.StockQuantity += itemDto.Quantity;

                    var detail = new PurchaseInvoiceDetail
                    {
                        PartID = itemDto.PartID,
                        Quantity = itemDto.Quantity,
                        UnitCost = itemDto.UnitCost,
                        Subtotal = itemDto.Quantity * itemDto.UnitCost,
                        Part = part
                    };

                    invoice.Details.Add(detail);
                    invoice.TotalAmount += detail.Subtotal;
                }

                context.PurchaseInvoices.Add(invoice);
                await context.SaveChangesAsync();
                await transaction.CommitAsync();

                return invoice;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}

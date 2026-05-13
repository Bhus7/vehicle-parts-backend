using Vehicle_partsAPI.DTOs;
using Vehicle_partsAPI.Models;

namespace Vehicle_partsAPI.Services
{
    public interface IPurchaseService
    {
        Task<PurchaseInvoice> CreatePurchaseAsync(PurchaseInvoiceCreateDto dto);
    }
}

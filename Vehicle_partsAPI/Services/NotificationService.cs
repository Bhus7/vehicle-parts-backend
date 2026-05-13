using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using vehicle_parts.Data;

namespace vehicle_parts.Services
{
    public class NotificationService : INotificationService
    {
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;

        public NotificationService(AppDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public async Task ProcessNotificationsAsync()
        {
            await CheckLowStockAsync();
            await CheckUnpaidCreditsAsync();
        }

        private async Task CheckLowStockAsync()
        {
            var lowStockProducts = await _context.Parts
                .Where(p => p.StockQuantity < 10)
                .ToListAsync();

            if (lowStockProducts.Any())
            {
                var adminEmail = "admin@vehicleparts.com"; // Default admin email
                var productList = string.Join("\n", lowStockProducts.Select(p => $"- {p.PartName} (Stock: {p.StockQuantity})"));
                
                await _emailService.SendEmailAsync(
                    adminEmail,
                    "LOW STOCK ALERT",
                    $"The following items are low in stock:\n\n{productList}\n\nPlease restock soon."
                );
            }
        }

        private async Task CheckUnpaidCreditsAsync()
        {
            var oneMonthAgo = DateTime.UtcNow.AddMonths(-1);
            
            var overdueInvoices = await _context.SalesInvoices
                .Include(i => i.Customer)
                .Where(i => i.PaymentStatus != "Paid" && i.SalesDate < oneMonthAgo)
                .ToListAsync();

            foreach (var invoice in overdueInvoices)
            {
                if (invoice.Customer != null && !string.IsNullOrEmpty(invoice.Customer.Email))
                {
                    await _emailService.SendEmailAsync(
                        invoice.Customer.Email,
                        "PAYMENT REMINDER: Unpaid Credits",
                        $"Dear {invoice.Customer.FullName},\n\nThis is a reminder that you have an unpaid credit of {invoice.FinalAmount:C} for the invoice created on {invoice.SalesDate:d}.\n\nPlease clear your dues as soon as possible."
                    );
                }
            }
        }
    }
}

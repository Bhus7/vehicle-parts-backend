using Microsoft.EntityFrameworkCore;
using vehicle_parts.Data;
using vehicle_parts.Dto.Response;
using vehicle_parts.Models;

namespace vehicle_parts.Repositories
{
    public interface IStaffRepository
    {
        Task<IEnumerable<CustomerSearchResponse>> SearchCustomersAsync(string query);
        Task<IEnumerable<CustomerReportResponse>> GetRegularCustomersAsync();
        Task<IEnumerable<CustomerReportResponse>> GetHighSpendersAsync();
        Task<IEnumerable<CustomerReportResponse>> GetPendingCreditsAsync();
    }

    public class StaffRepository : IStaffRepository
    {
        private readonly AppDbContext _context;

        public StaffRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CustomerSearchResponse>> SearchCustomersAsync(string query)
        {
            var users = _context.Users
                .Include(u => u.Vehicles)
                .Include(u => u.SalesInvoices)
                .AsQueryable();

            if (!string.IsNullOrEmpty(query))
            {
                users = users.Where(u =>
                    u.FullName.Contains(query) ||
                    u.Phone.Contains(query) ||
                    u.UserID.ToString() == query ||
                    u.Vehicles.Any(v => v.VehicleNumber.Contains(query))
                );
            }

            return await users.Select(u => new CustomerSearchResponse
            {
                UserID = u.UserID,
                FullName = u.FullName ?? string.Empty,
                Phone = u.Phone ?? string.Empty,
                Email = u.Email,
                VehicleNumber = u.Vehicles.OrderByDescending(v => v.VehicleID).Select(v => v.VehicleNumber).FirstOrDefault(),
                Brand = u.Vehicles.OrderByDescending(v => v.VehicleID).Select(v => v.Brand).FirstOrDefault(),
                Model = u.Vehicles.OrderByDescending(v => v.VehicleID).Select(v => v.Model).FirstOrDefault(),
                LatestInvoiceAmount = u.SalesInvoices.OrderByDescending(s => s.SalesDate).Select(s => s.FinalAmount).FirstOrDefault(),
                PaymentStatus = u.SalesInvoices.OrderByDescending(s => s.SalesDate).Select(s => s.PaymentStatus).FirstOrDefault()
            }).ToListAsync();
        }

        public async Task<IEnumerable<CustomerReportResponse>> GetRegularCustomersAsync()
        {
            return await _context.Users
                .Where(u => u.SalesInvoices.Count >= 2)
                .Select(u => new CustomerReportResponse
                {
                    UserID = u.UserID,
                    FullName = u.FullName ?? string.Empty,
                    TotalOrders = u.SalesInvoices.Count,
                    TotalSpent = u.SalesInvoices.Sum(s => s.FinalAmount),
                    RemainingBalance = u.SalesInvoices.Sum(s => s.FinalAmount) - u.SalesInvoices.SelectMany(s => s.Payments).Sum(p => p.AmountPaid),
                    DueDate = u.SalesInvoices.OrderByDescending(s => s.DueDate).Select(s => s.DueDate).FirstOrDefault()
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<CustomerReportResponse>> GetHighSpendersAsync()
        {
            return await _context.Users
                .Select(u => new CustomerReportResponse
                {
                    UserID = u.UserID,
                    FullName = u.FullName ?? string.Empty,
                    TotalOrders = u.SalesInvoices.Count,
                    TotalSpent = u.SalesInvoices.Sum(s => s.FinalAmount),
                    RemainingBalance = u.SalesInvoices.Sum(s => s.FinalAmount) - u.SalesInvoices.SelectMany(s => s.Payments).Sum(p => p.AmountPaid),
                    DueDate = u.SalesInvoices.OrderByDescending(s => s.DueDate).Select(s => s.DueDate).FirstOrDefault()
                })
                .OrderByDescending(r => r.TotalSpent)
                .Take(20) // Limit to top 20 high spenders
                .ToListAsync();
        }

        public async Task<IEnumerable<CustomerReportResponse>> GetPendingCreditsAsync()
        {
            // Pending Credits: Users who have a total remaining balance > 0
            return await _context.Users
                .Where(u => u.SalesInvoices.Sum(s => s.FinalAmount) - u.SalesInvoices.SelectMany(s => s.Payments).Sum(p => p.AmountPaid) > 0)
                .Select(u => new CustomerReportResponse
                {
                    UserID = u.UserID,
                    FullName = u.FullName ?? string.Empty,
                    TotalOrders = u.SalesInvoices.Count,
                    TotalSpent = u.SalesInvoices.Sum(s => s.FinalAmount),
                    RemainingBalance = u.SalesInvoices.Sum(s => s.FinalAmount) - u.SalesInvoices.SelectMany(s => s.Payments).Sum(p => p.AmountPaid),
                    DueDate = u.SalesInvoices.OrderByDescending(s => s.DueDate).Select(s => s.DueDate).FirstOrDefault()
                })
                .OrderByDescending(r => r.RemainingBalance)
                .ToListAsync();
        }
    }
}

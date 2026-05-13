using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using vehicle_parts.Data;
using vehicle_parts.Models;
using vehicle_parts.Dto;
using vehicle_parts.Dto.Requests;
using vehicle_parts.Services;

namespace vehicle_parts.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Tags("1. Staff - Customer & Vehicle Management")]
    // [Authorize(Roles = "ADMIN")] // Uncomment when Authentication is fully configured
    public class StaffController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IStaffService _staffService;

        public StaffController(AppDbContext context, IStaffService staffService)
        {
            _context = context;
            _staffService = staffService;
        }

        // FEATURE 6: Staff can register new customers with vehicle details
        [HttpPost("register-customer")]
        public IActionResult RegisterCustomer(RegisterCustomerDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.VehicleNumber))
            {
                return BadRequest("Email and Vehicle Number are required.");
            }

            // Start a transaction to ensure both User and Vehicle are created or neither
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    // 1. Create the User (Customer)
                    var user = new User
                    {
                        FullName = dto.FullName,
                        Email = dto.Email,
                        Phone = dto.Phone,
                        PasswordHash = dto.Password, // In a real app, hash this!
                        Address = dto.Address,
                        RoleID = 3, // RoleID 3 is "Customer"
                        CreatedDate = DateTime.UtcNow,
                        Status = "Active"
                    };

                    _context.Users.Add(user);
                    _context.SaveChanges(); // Save to get the UserID

                    // 2. Create the Vehicle
                    var vehicle = new Vehicle
                    {
                        UserID = user.UserID,
                        VehicleNumber = dto.VehicleNumber,
                        Brand = dto.Brand,
                        Model = dto.Model,
                        Year = dto.Year,
                        VehicleType = dto.VehicleType,
                        ConditionNotes = dto.ConditionNotes
                    };

                    _context.Vehicles.Add(vehicle);
                    _context.SaveChanges();

                    transaction.Commit();

                    return Ok(new
                    {
                        Message = "Customer and Vehicle registered successfully",
                        CustomerID = user.UserID,
                        VehicleID = vehicle.VehicleID
                    });
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return StatusCode(500, $"Internal server error: {ex.Message}");
                }
            }
        }

        // FEATURE 8 (Partial): View Customer Details
        [HttpGet("customer/{id}")]
        public IActionResult GetCustomerDetails(int id)
        {
            var customer = _context.Users
                .Where(u => u.UserID == id && u.RoleID == 3)
                .Select(u => new
                {
                    u.UserID,
                    u.FullName,
                    u.Email,
                    u.Phone,
                    u.Address,
                    u.CreatedDate,
                    Vehicles = _context.Vehicles.Where(v => v.UserID == u.UserID).ToList(),
                    Appointments = _context.Appointments.Where(a => a.UserID == u.UserID).OrderByDescending(a => a.AppointmentDate).ToList(),
                    Sales = _context.SalesInvoices.Where(s => s.UserID == u.UserID).OrderByDescending(s => s.SalesDate).ToList()
                })
                .FirstOrDefault();

            if (customer == null) return NotFound("Customer not found");

            return Ok(customer);
        }

        // FEATURE 7: Staff can sell vehicle parts and create sales invoices
        [HttpPost("create-sale")]
        [Tags("4. Sales & Invoices")]
        public IActionResult CreateSale(SaleCreateDto dto)
        {
            if (dto.Items == null || !dto.Items.Any())
                return BadRequest("No parts selected for sale.");

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    decimal totalAmount = 0;
                    var invoiceDetails = new List<SalesInvoiceDetail>();

                    // 1. Process each item
                    foreach (var item in dto.Items)
                    {
                        var part = _context.Parts.Find(item.PartID);
                        if (part == null) return NotFound($"Part with ID {item.PartID} not found.");

                        if (part.StockQuantity < item.Quantity)
                            return BadRequest($"Insufficient stock for {part.PartName}. Available: {part.StockQuantity}");

                        decimal subtotal = part.UnitPrice * item.Quantity;
                        totalAmount += subtotal;

                        // Deduct Stock
                        part.StockQuantity -= item.Quantity;

                        // Create Detail Record
                        invoiceDetails.Add(new SalesInvoiceDetail
                        {
                            PartID = part.PartID,
                            Quantity = item.Quantity,
                            UnitPrice = part.UnitPrice,
                            Subtotal = subtotal
                        });

                        // FEATURE 15: Low Stock Notification
                        if (part.StockQuantity < 10)
                        {
                            _context.Notifications.Add(new Notification
                            {
                                UserID = 1, // Notify Admin (Assuming UserID 1 is Admin)
                                NotificationType = "Low Stock",
                                Message = $"Low stock alert: {part.PartName} has only {part.StockQuantity} left.",
                                CreatedDate = DateTime.UtcNow
                            });
                        }
                    }

                    // 2. FEATURE 16: Loyalty Discount (10% if > 5000)
                    decimal discount = 0;
                    if (totalAmount > 5000)
                    {
                        discount = totalAmount * 0.1m;
                    }

                    decimal finalAmount = totalAmount - discount;

                    // 3. Create Invoice
                    var invoice = new SalesInvoice
                    {
                        UserID = dto.UserID,
                        SalesDate = DateTime.UtcNow,
                        TotalAmount = totalAmount,
                        DiscountAmount = discount,
                        FinalAmount = finalAmount,
                        PaymentStatus = "Paid",
                        Details = invoiceDetails
                    };

                    _context.SalesInvoices.Add(invoice);
                    _context.SaveChanges();

                    // 4. Record Payment
                    var payment = new Payment
                    {
                        SalesInvoiceID = invoice.SalesInvoiceID,
                        PaymentDate = DateTime.UtcNow,
                        AmountPaid = finalAmount,
                        PaymentMethod = dto.PaymentMethod,
                        RemainingBalance = 0
                    };
                    _context.Payments.Add(payment);
                    _context.SaveChanges();

                    transaction.Commit();

                    return Ok(new
                    {
                        InvoiceID = invoice.SalesInvoiceID,
                        Total = totalAmount,
                        Discount = discount,
                        Final = finalAmount,
                        Message = "Sale completed successfully"
                    });
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return StatusCode(500, $"Internal server error: {ex.Message}");
                }
            }
        }

        // TEMPORARY: Add Part (For testing Feature 7)
        [HttpPost("add-part")]
        public IActionResult AddPart(Part part)
        {
            _context.Parts.Add(part);
            _context.SaveChanges();
            return Ok(part);
        }

        // TEMPORARY: Get All Parts
        [HttpGet("parts")]
        public IActionResult GetAllParts()
        {
            return Ok(_context.Parts.ToList());
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

        [HttpGet("find-all-customers")]
        public IActionResult SearchCustomers(string? query)
        {
            try
            {
                var customers = _context.Users.Where(u => u.RoleID == 3).AsQueryable();

                if (!string.IsNullOrWhiteSpace(query))
                {
                    string q = query.ToLower();
                    customers = customers.Where(u => 
                        (u.FullName != null && u.FullName.ToLower().Contains(q)) || 
                        (u.Phone != null && u.Phone.Contains(q))
                    );
                }

                var results = customers
                    .OrderByDescending(u => u.UserID)
                    .Take(50)
                    .Select(u => new {
                        u.UserID,
                        u.FullName,
                        u.Email,
                        u.Phone,
                        Vehicles = _context.Vehicles.Where(v => v.UserID == u.UserID).Select(v => v.VehicleNumber).ToList()
                    })
                    .ToList();

                return Ok(results);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Error: {ex.Message}");
            }
        }

        // FEATURE 9: System should maintain service history for each customer
        [HttpGet("customer-history/{id}")]
        [Tags("1. Staff - Customer & Vehicle Management")]
        public IActionResult GetCustomerHistory(int id)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserID == id && u.RoleID == 3);
            if (user == null) return NotFound("Customer not found");

            var history = new
            {
                CustomerName = user.FullName,
                Appointments = _context.Appointments
                    .Where(a => a.UserID == id)
                    .OrderByDescending(a => a.AppointmentDate)
                    .Select(a => new {
                        a.AppointmentID,
                        a.AppointmentDate,
                        a.ServiceType,
                        a.Status,
                        a.Notes
                    }).ToList(),
                Sales = _context.SalesInvoices
                    .Where(s => s.UserID == id)
                    .OrderByDescending(s => s.SalesDate)
                    .Select(s => new {
                        s.SalesInvoiceID,
                        s.SalesDate,
                        s.FinalAmount,
                        s.PaymentStatus,
                        ItemsCount = _context.SalesInvoiceDetails.Count(d => d.SalesInvoiceID == s.SalesInvoiceID)
                    }).ToList()
            };

            return Ok(history);
        }

        // FEATURE 11: Staff can generate customer-related reports
        [HttpGet("reports/customers")]
        [Tags("1. Staff - Customer & Vehicle Management")]
        public IActionResult GetCustomerReports()
        {
            // 1. High Spenders (Top 5 by total sales)
            var highSpenders = _context.Users
                .Where(u => u.RoleID == 3)
                .Select(u => new {
                    u.UserID,
                    u.FullName,
                    TotalSpent = _context.SalesInvoices.Where(s => s.UserID == u.UserID).Sum(s => s.FinalAmount)
                })
                .OrderByDescending(x => x.TotalSpent)
                .Take(5)
                .ToList();

            // 2. Regulars (Top 5 by frequency of visits/appointments)
            var regularCustomers = _context.Users
                .Where(u => u.RoleID == 3)
                .Select(u => new {
                    u.UserID,
                    u.FullName,
                    VisitCount = _context.Appointments.Count(a => a.UserID == u.UserID) + 
                                 _context.SalesInvoices.Count(s => s.UserID == u.UserID)
                })
                .OrderByDescending(x => x.VisitCount)
                .Take(5)
                .ToList();

            // 3. Pending Credits (Customers with unpaid invoices)
            var pendingPayments = _context.SalesInvoices
                .Where(s => s.PaymentStatus != "Paid")
                .Select(s => new {
                    s.SalesInvoiceID,
                    s.UserID,
                    CustomerName = s.Customer.FullName,
                    s.FinalAmount,
                    s.DueDate
                })
                .ToList();

            return Ok(new {
                HighSpenders = highSpenders,
                Regulars = regularCustomers,
                PendingPayments = pendingPayments
            });
        }

        // Dashboard Stats
        [HttpGet("dashboard-stats")]
        [Tags("1. Staff - Customer & Vehicle Management")]
        public IActionResult GetDashboardStats()
        {
            var stats = new
            {
                TotalCustomers = _context.Users.Count(u => u.RoleID == 3),
                TotalSales = _context.SalesInvoices.Sum(s => s.FinalAmount),
                PendingAppointments = _context.Appointments.Count(a => a.Status != "Completed"),
                LowStockAlerts = _context.Parts.Count(p => p.StockQuantity < 10)
            };

            return Ok(stats);
        }

        // Get single invoice details
        [HttpGet("invoice/{id}")]
        [Tags("4. Sales & Invoices")]
        public IActionResult GetInvoice(int id)
        {
            var invoice = _context.SalesInvoices
                .Where(s => s.SalesInvoiceID == id)
                .Select(s => new {
                    s.SalesInvoiceID,
                    s.SalesDate,
                    s.TotalAmount,
                    s.DiscountAmount,
                    s.FinalAmount,
                    s.PaymentStatus,
                    CustomerName = s.Customer != null ? s.Customer.FullName : "Unknown",
                    CustomerEmail = s.Customer != null ? s.Customer.Email : "N/A",
                    CustomerPhone = s.Customer != null ? s.Customer.Phone : "N/A",
                    CustomerAddress = s.Customer != null ? s.Customer.Address : "N/A",
                    Items = _context.SalesInvoiceDetails
                        .Where(d => d.SalesInvoiceID == s.SalesInvoiceID)
                        .Select(d => new {
                            PartName = d.Part != null ? d.Part.PartName : "Deleted Part",
                            d.Quantity,
                            d.UnitPrice,
                            d.Subtotal
                        }).ToList()
                })
                .FirstOrDefault();

            if (invoice == null) return NotFound("Invoice not found in database");

            return Ok(invoice);

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

using Microsoft.AspNetCore.Mvc;
using vehicle_parts.Data;
using vehicle_parts.Models;
using vehicle_parts.Dto;
using System;
using System.Linq;

namespace vehicle_parts.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Tags("1. Staff - Customer & Vehicle Management")]
    public class StaffController : ControllerBase
    {
        private readonly AppDbContext _context;

        public StaffController(AppDbContext context)
        {
            _context = context;
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
                    Vehicles = _context.Vehicles.Where(v => v.UserID == u.UserID).ToList()
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
        [Tags("5. Inventory (Testing)")]
        public IActionResult AddPart(Part part)
        {
            _context.Parts.Add(part);
            _context.SaveChanges();
            return Ok(part);
        }

        // TEMPORARY: Get All Parts
        [HttpGet("parts")]
        [Tags("5. Inventory (Testing)")]
        public IActionResult GetAllParts()
        {
            return Ok(_context.Parts.ToList());
        }
    }
}

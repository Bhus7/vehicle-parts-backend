using Microsoft.EntityFrameworkCore;
using vehicle_parts.Models;

namespace vehicle_parts.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<SalesInvoice> SalesInvoices { get; set; }
        public DbSet<SalesInvoiceDetail> SalesInvoiceDetails { get; set; }
        public DbSet<Payment> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships and precision if needed
            modelBuilder.Entity<SalesInvoice>()
                .Property(s => s.TotalAmount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<SalesInvoice>()
                .Property(s => s.DiscountAmount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<SalesInvoice>()
                .Property(s => s.FinalAmount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<SalesInvoiceDetail>()
                .Property(s => s.UnitPrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<SalesInvoiceDetail>()
                .Property(s => s.Subtotal)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Payment>()
                .Property(p => p.AmountPaid)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Payment>()
                .Property(p => p.RemainingBalance)
                .HasPrecision(18, 2);
        }
    }
}

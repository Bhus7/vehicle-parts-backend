using Microsoft.EntityFrameworkCore;
using vehicle_parts.Models;

namespace vehicle_parts.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<Customer> Customers { get; set; }

        public DbSet<Appointment> Appointments { get; set; }

        public DbSet<PartRequest> PartRequests { get; set; }


    }
}

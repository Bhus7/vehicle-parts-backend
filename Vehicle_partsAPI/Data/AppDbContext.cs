using Microsoft.EntityFrameworkCore;
using Vehicle_partsAPI.Models;

namespace Vehicle_partsAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
    }
}
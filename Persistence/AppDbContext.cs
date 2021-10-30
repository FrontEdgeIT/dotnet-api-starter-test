using Microsoft.EntityFrameworkCore;

namespace dotnet_api_test.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> opt) : base(opt)
        {
        }

        public DbSet<Dish> Dishes { get; set; } = null!;
    }
}
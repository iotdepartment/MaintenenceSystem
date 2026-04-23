using Microsoft.EntityFrameworkCore;

namespace MaintenenceSystem.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }
        public DbSet<OrdenesMaquinados> OrdenesMaquinados { get; set; }

        public DbSet<Areas> Areas { get; set; }

        public DbSet<Empleados> Empleados { get; set; }
    }

}

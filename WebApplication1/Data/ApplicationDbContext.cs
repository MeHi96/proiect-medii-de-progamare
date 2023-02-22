using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        public DbSet<travelling.agency.Models.Vacation> Vacation { get; set; }
        public DbSet<travelling.agency.Models.Destination> Destination { get; set; }
        public DbSet<travelling.agency.Models.Bookings> Bookings { get; set; }

        public DbSet<WebApplication1.Models.Favorites> Favorites { get; set; }

    }
}
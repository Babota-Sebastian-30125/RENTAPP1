using Microsoft.EntityFrameworkCore;
using RentApp.Models;
using RentApp.Server.Models;

namespace RentApp.Data
{
    public class RentDbContext : DbContext
    {
        public RentDbContext(DbContextOptions<RentDbContext> options)
       : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Rental> Rentals { get; set;}
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Favorite> Favorites { get; set; }


    }
}
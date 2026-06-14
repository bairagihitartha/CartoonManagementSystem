using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CartoonManagementSystem.Models;

namespace CartoonManagementSystem.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Cartoon> Cartoons { get; set; }
        public DbSet<Rating> Ratings { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Seed initial requested data
            builder.Entity<Cartoon>().HasData(
                new Cartoon { Id = 1, Name = "Tom & Jerry", Description = "The iconic cat and mouse chase.", Genre = "Slapstick Comedy", ImagePath = "tom-jerry.jpg" },
                new Cartoon { Id = 2, Name = "Oggy & the Cockroaches", Description = "Oggy fights off three troublesome roaches.", Genre = "Comedy", ImagePath = "oggy.jpg" },
                new Cartoon { Id = 3, Name = "Ninja Hattori", Description = "A little ninja moves in with a regular family.", Genre = "Action/Comedy", ImagePath = "ninja-hattori.jpg" },
                new Cartoon { Id = 4, Name = "Roll No 21", Description = "Modern take on Krishna and Kansa in a school setting.", Genre = "Mythological/Comedy", ImagePath = "rollno21.jpg" },
                new Cartoon { Id = 5, Name = "Honey Bunny ka Jholmaal", Description = "The crazy adventures of two pet cats.", Genre = "Comedy", ImagePath = "honey-bunny.jpg" }
            );
        }
    }
}
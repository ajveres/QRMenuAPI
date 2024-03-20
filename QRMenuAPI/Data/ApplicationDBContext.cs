using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using QRMenuAPI.Models;
using Microsoft.AspNetCore.Identity;

namespace QRMenuAPI.Data
{
	public class ApplicationDBContext: IdentityDbContext<ApplicationUser>
	{
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
        {
		}
        public DbSet<Company>? Companies { get; set; }
        public DbSet<State>? States { get; set; }
        public DbSet<Restaurant>? Restaurants { get; set; }
        public DbSet<Category>? Categories { get; set; }
        public DbSet<Food>? Foods { get; set; }
        public DbSet<RestaurantUser>? RestaurantUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ApplicationUser>().HasOne(u => u.State).WithMany().OnDelete(DeleteBehavior.NoAction);
            

            modelBuilder.Entity<Restaurant>().HasOne(r => r.State).WithMany().OnDelete(DeleteBehavior.NoAction);
          

            modelBuilder.Entity<Category>().HasOne(c => c.State).WithMany().OnDelete(DeleteBehavior.NoAction);
            

            modelBuilder.Entity<Food>().HasOne(f => f.State).WithMany().OnDelete(DeleteBehavior.NoAction);

            
            modelBuilder.Entity<RestaurantUser>().HasOne(r => r.Restaurant).WithMany().OnDelete(DeleteBehavior.NoAction);


            modelBuilder.Entity<RestaurantUser>().HasKey(r => new { r.RestaurantId, r.UserId });

            base.OnModelCreating(modelBuilder);

        }


    }
}


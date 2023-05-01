using System;
using Microsoft.EntityFrameworkCore;
using MVCModel.Models;

namespace MVCModel.Data
{
    public class MVCModelDbContext : DbContext
    {
        public MVCModelDbContext(DbContextOptions<MVCModelDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure the User entity
            modelBuilder.Entity<User>().HasKey(u => u.Id);
            modelBuilder.Entity<User>().Property(u => u.Name).IsRequired();
            modelBuilder.Entity<User>().Property(u => u.Password).IsRequired();
            modelBuilder.Entity<User>().Property(u => u.Email).IsRequired();
        }

    }
}


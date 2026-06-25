using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
//using SalesXZone.Core.Entities;

namespace SalesXZone.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Product entity
            //modelBuilder.Entity<Product>(entity =>
            //{
            //    entity.HasKey(e => e.Id);
            //    entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            //    entity.Property(e => e.Description).HasMaxLength(1000);
            //    entity.Property(e => e.Price).HasPrecision(18, 2);
            //    entity.Property(e => e.Sku).HasMaxLength(50);
            //    entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            //});
        }
    }
}

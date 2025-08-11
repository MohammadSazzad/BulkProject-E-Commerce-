using BulkProject.Models;
using Microsoft.EntityFrameworkCore;

namespace BulkProject.Data
{
    public class BulkDbContext : DbContext
    {
        public BulkDbContext(DbContextOptions<BulkDbContext> options) : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Electronics", DisplayOrder = 1 },
                new Category { Id = 2, Name = "Books", DisplayOrder = 2 },
                new Category { Id = 3, Name = "Clothing", DisplayOrder = 3 }
            );
        }
    }
}

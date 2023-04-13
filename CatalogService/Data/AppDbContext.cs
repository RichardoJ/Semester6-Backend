using CatalogService.Model;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> opt) : base(opt)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Computer Science"},
            new Category { Id = 2, Name = "Business"}
                );

            modelBuilder.Entity<Paper>()
                .HasIndex(b => b.Title)
                .IsUnique();
        }

        public DbSet<Paper> papers { get; set; }
        public DbSet<Category> categories { get;set; }
    }
}

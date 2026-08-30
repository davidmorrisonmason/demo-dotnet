using Demo.Model.Domain;
using Microsoft.EntityFrameworkCore;
namespace Demo.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Category>(category =>
        {
            category.ToTable("Categories");

            category.HasKey(c => c.Id);

            category.Property(c => c.Name)
                .HasMaxLength(255);

            category.HasMany(c => c.Products)
                .WithOne()
                .HasForeignKey(c => c.CategoryId);

            category.HasMany(c => c.SubCategories)
                .WithOne()
                .HasForeignKey(c => c.ParentCategoryId);
        });

        modelBuilder.Entity<Product>(product =>
        {
            product.ToTable("Products");

            product.HasKey(c => c.Id);

            product.Property(c => c.Name)
                .HasMaxLength(255);

            product.Property(c => c.Price);
        });
    }
}

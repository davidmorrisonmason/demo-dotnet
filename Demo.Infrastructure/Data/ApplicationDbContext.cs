using Demo.Model.Domain;
using Demo.Model.Domain.Checkout;
using Microsoft.EntityFrameworkCore;
namespace Demo.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Basket> Baskets { get; set; }

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

            product.HasKey(p => p.Id);

            product.Property(p => p.Name)
                .HasMaxLength(255);

            product.Property(p => p.Price);
        });

        modelBuilder.Entity<BasketItem>(basketItem =>
        {
            basketItem.ToTable("BasketItems");

            basketItem.HasKey(b => b.Id);

            basketItem.Property(b => b.ProductId);
            basketItem.Property(b => b.Quantity);

            basketItem.HasOne(b => b.Product)
                .WithMany()
                .HasForeignKey(b => b.ProductId);
        });

        modelBuilder.Entity<Basket>(basket =>
        {
            basket.ToTable("Baskets");

            basket.HasKey(b => b.Id);

            basket.HasMany(b => b.BasketItems)
                .WithOne()
                .HasForeignKey(i => i.BasketId);
        });

    }
}

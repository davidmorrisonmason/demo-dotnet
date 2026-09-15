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
    public DbSet<CheckoutCompletion> CheckoutCompletions { get; set; }
    public DbSet<Client> Clients { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Category>(category =>
        {
            category.ToTable("Categories");

            category.HasKey(c => c.Id);

            category.Property(c => c.Name)
                .HasMaxLength(255);

            category.HasOne<Client>()
                .WithMany()
                .HasForeignKey(c => c.ClientId);

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

        modelBuilder.Entity<CheckoutCompletion>(checkoutCompletion =>
        {
            checkoutCompletion.ToTable("CheckoutCompletions");

            checkoutCompletion.HasKey(c => c.Id);

            checkoutCompletion.Property(c => c.Recipient)
                .HasMaxLength(255);

            checkoutCompletion.Property(c => c.AddressLine1)
                .HasMaxLength(255);

            checkoutCompletion.Property(c => c.AddressLine2)
                .HasMaxLength(255);

            checkoutCompletion.Property(c => c.AddressLine3)
                .HasMaxLength(255);

            checkoutCompletion.Property(c => c.AddressLine4)
                .HasMaxLength(255);

            checkoutCompletion.Property(c => c.City)
                .HasMaxLength(255);

            checkoutCompletion.Property(c => c.PostCode)
                .HasMaxLength(255);

            checkoutCompletion.HasOne(c => c.Basket)
                .WithOne()
                .HasForeignKey<CheckoutCompletion>(c => c.BasketId);
        });

        modelBuilder.Entity<Client>(client =>
        {
            client.ToTable("Clients");

            client.HasKey(c => c.Id);

            client.Property(c => c.Name)
                .HasMaxLength(255);

            client.Property(c => c.ApiKey)
                .HasMaxLength(255);
        });

    }
}

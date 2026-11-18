using si730pc2u201621873.API.Sale.Domain.Model.Aggregates;
using si730pc2u201621873.API.Sale.Domain.Model.ValueObjects;
using si730pc2u201621873.API.Shared.Infrastructure.Persistence.EFC.Configuration.Extensions;
using si730pc2u201621873.API.Shared.Infrastructure.Persistence.EFC.Interceptors;
using Microsoft.EntityFrameworkCore;

namespace si730pc2u201621873.API.Shared.Infrastructure.Persistence.EFC.Configuration;

/// <summary>
///     Application database context
/// </summary>
public class AppDbContext(DbContextOptions options) : DbContext(options)
{
    /// <inheritdoc />
    protected override void OnConfiguring(DbContextOptionsBuilder builder)
    {
        // Apply audit timestamp interceptor for all IAuditableEntity implementations
        builder.AddInterceptors(new AuditableEntityInterceptor());
        base.OnConfiguring(builder);
    }

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<PurchaseOrder>().HasKey(p => p.Id);
        builder.Entity<PurchaseOrder>().Property(p => p.Id).IsRequired().ValueGeneratedOnAdd();

        builder.Entity<PurchaseOrder>()
            .Property(p => p.Customer)
            .HasConversion(valueObject => valueObject.Value, value => new Customer(value))
            .HasMaxLength(50)
            .IsRequired();

        builder.Entity<PurchaseOrder>()
            .Property(p => p.PoNumber)
            .HasConversion(valueObject => valueObject.Value, value => new PoNumber(value))
            .HasMaxLength(255)
            .IsRequired();

        builder.Entity<PurchaseOrder>()
            .Property(p => p.FabricId)
            .HasConversion(valueObject => valueObject.Value, value => new FabricId(value))
            .IsRequired();

        builder.Entity<PurchaseOrder>()
            .Property(p => p.Vendor)
            .HasConversion(valueObject => valueObject.Value, value => new Vendor(value))
            .HasMaxLength(50)
            .IsRequired();

        builder.Entity<PurchaseOrder>()
            .Property(p => p.ShipTo)
            .HasConversion(valueObject => valueObject.Value, value => new ShipTo(value))
            .HasMaxLength(255)
            .IsRequired();

        builder.Entity<PurchaseOrder>().Property(p => p.Quantity).IsRequired();

        // A customer cannot register two purchase orders with the same P.O. Number
        builder.Entity<PurchaseOrder>()
            .HasIndex(p => new { p.Customer, p.PoNumber })
            .IsUnique();

        builder.UseSnakeCaseNamingConvention();
    }
}
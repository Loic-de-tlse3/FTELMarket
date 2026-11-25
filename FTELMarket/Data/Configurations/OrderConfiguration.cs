
using FTELMarket;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class orderConfiguration : IEntityTypeConfiguration<OrderRequest>
{
    public void Configure(EntityTypeBuilder<OrderRequest> builder)
    {
        builder
            .HasIndex(order => order.Id).IsUnique();

        builder
            .Property(order => order.OrderNumber).IsRequired();
        
        builder
            .Property(order => order.CreatedAt).IsRequired();

        builder
            .Property(order => order.Status).IsRequired();

        builder
            .Property(order => order.Id)
            .ValueGeneratedNever();
        
        builder.HasOne(order => order.Client)
            .WithMany()
            .HasForeignKey(order => order.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(order => order.Lines)
            .WithOne(line => line.Order)
            .HasForeignKey(line => line.OrderId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
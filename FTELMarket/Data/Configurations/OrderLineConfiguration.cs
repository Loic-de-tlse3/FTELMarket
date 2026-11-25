using FTELMarket;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class OrderLineConfiguration : IEntityTypeConfiguration<OrderLine>
{
    public void Configure(EntityTypeBuilder<OrderLine> builder)
    {
        builder
            .Property(l => l.ProductReference)
            .IsRequired();

        builder
            .Property(l => l.Quantity)
            .IsRequired();

        builder
            .Property(l => l.UnitPrice)
            .IsRequired();
        
        builder
            .Property(l => l.Id)
            .ValueGeneratedNever();

        builder.HasOne(l => l.Order)
            .WithMany(order => order.Lines)
            .HasForeignKey(l => l.OrderId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
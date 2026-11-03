using FTELMarket;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder
            .Property(product => product.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder
            .Property(product => product.Description)
            .HasMaxLength(1000);

        builder
            .Property(product => product.Price)
            .IsRequired()
            .HasPrecision(18, 2);

        builder
            .Property(product => product.Stock)
            .IsRequired();

        builder
            .Property(product => product.CategoryId)
            .IsRequired();

        builder.HasOne(product => product.Category)
            .WithMany(category => category.Products)
            .HasForeignKey(Product => Product.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.ToTable(t =>
        {
            t.HasCheckConstraint("CK_Products_Name_NotEmpty", "length(trim(\"Name\")) >= 1");
            t.HasCheckConstraint("CK_Products_Price_Min", "\"Price\" >= 0.01");
            t.HasCheckConstraint("CK_Products_Stock_Min", "\"Stock\" >= 0");
        });
    }
}
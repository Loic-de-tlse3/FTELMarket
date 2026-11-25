using FTELMarket;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder
            .Property(category => category.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder
            .HasIndex(category => category.Name).IsUnique();

        builder.ToTable(t =>
        {  
            t.HasCheckConstraint("CK_Category_Name_NotEmpty", "length(trim(\"Name\")) >= 1");
        });

        builder.HasData(
            new Category { Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), Name = "PlayStation 5" },
            new Category { Id = Guid.Parse("22222222-2222-2222-2222-222222222222"), Name = "Nintendo Switch" },
            new Category { Id = Guid.Parse("33333333-3333-3333-3333-333333333333"), Name = "Xbox Series X" },
            new Category { Id = Guid.Parse("44444444-4444-4444-4444-444444444444"), Name = "PC" }
        );
    }
}
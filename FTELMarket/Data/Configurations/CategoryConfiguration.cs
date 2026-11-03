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
            new Category { Id = 1, Name = "PlayStation 5" },
            new Category { Id = 2, Name = "Nintendo Switch" },
            new Category { Id = 3, Name = "Xbox Series X" },
            new Category { Id = 4, Name = "PC" }
        );
    }
}
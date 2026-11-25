using FTELMarket;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ClientConfiguration : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        builder
            .HasIndex(client => client.Id).IsUnique();

        builder
            .Property(client => client.FirstName).IsRequired();

        builder
            .Property(client => client.LastName).IsRequired();

        builder
            .Property(client => client.Email).IsRequired();

        builder
            .Property(client => client.City).IsRequired();

        builder
            .Property(client => client.Id)
            .ValueGeneratedNever();
    }
}
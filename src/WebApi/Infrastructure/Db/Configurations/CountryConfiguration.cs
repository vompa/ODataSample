namespace OData.Sample.WebApi.Infrastructure.Db.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using OData.Sample.WebApi.Domain.Entities;

public class CountryConfiguration : IEntityTypeConfiguration<Country>
{
	public void Configure(EntityTypeBuilder<Country> builder)
	{
		if (builder == null)
		{
			return;
		}

		builder
			.HasKey(x => x.Id);

		builder
			.Property(x => x.Id)
			.ValueGeneratedOnAdd();

		builder
			.Property(x => x.DisplayName)
			.IsUnicode(true)
			.HasMaxLength(200)
			.IsRequired();

		builder
			.Property(x => x.DisplayNameGER)
			.IsUnicode(true)
			.HasMaxLength(200)
			.IsRequired();

		builder
			.Property(x => x.EU)
			.IsUnicode(true)
			.HasMaxLength(3)
			.IsRequired(false);

		builder
			.Property(x => x.EWR)
			.IsUnicode(true)
			.HasMaxLength(3)
			.IsRequired(false);

		builder
			.Property(x => x.ISO2)
			.IsUnicode(true)
			.HasMaxLength(10)
			.IsRequired(false);

		builder
			.Property(x => x.ISO3)
			.IsUnicode(true)
			.HasMaxLength(10)
			.IsRequired(false);

		builder
			.Property(x => x.Name)
			.IsUnicode(true)
			.HasMaxLength(200)
			.IsRequired();

		builder
			.Property(x => x.NameGER)
			.IsUnicode(true)
			.HasMaxLength(200)
			.IsRequired();

		builder
			.HasOne(x => x.WorldRegion)
			.WithMany(e => e.Countries)
			.HasForeignKey(c => c.WorldRegionId)
			.OnDelete(DeleteBehavior.NoAction);

		builder
			.HasOne(x => x.CountryRegion)
			.WithMany(e => e.Countries)
			.HasForeignKey(c => c.CountryRegionId)
			.OnDelete(DeleteBehavior.NoAction);
	}
}

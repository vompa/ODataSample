namespace OData.Sample.WebApi.Infrastructure.Db.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using OData.Sample.WebApi.Domain.Entities;

public class CountryRegionConfiguration : IEntityTypeConfiguration<CountryRegion>
{
	public void Configure(EntityTypeBuilder<CountryRegion> builder)
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
			.Property(x => x.ISO)
			.IsUnicode(true)
			.HasMaxLength(10)
			.IsRequired();

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
			.WithMany(e => e.CountryRegions)
			.HasForeignKey(c => c.WorldRegionId)
			.OnDelete(DeleteBehavior.NoAction);

		builder
			.HasMany(x => x.Countries)
			.WithOne(y => y.CountryRegion)
			.HasForeignKey(c => c.CountryRegionId)
			.OnDelete(DeleteBehavior.NoAction);
	}
}

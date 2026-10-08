namespace OData.Sample.WebApi.Infrastructure.Db.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using OData.Sample.WebApi.Domain.Entities;

public class WorldRegionConfiguration : IEntityTypeConfiguration<WorldRegion>
{
	public void Configure(EntityTypeBuilder<WorldRegion> builder)
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
			.HasMany(x => x.CountryRegions)
			.WithOne(y => y.WorldRegion)
			.HasForeignKey(c => c.WorldRegionId)
			.OnDelete(DeleteBehavior.NoAction);

		builder
			.HasMany(x => x.Countries)
			.WithOne(y => y.WorldRegion)
			.HasForeignKey(c => c.WorldRegionId)
			.OnDelete(DeleteBehavior.NoAction);
	}
}

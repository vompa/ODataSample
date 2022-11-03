namespace OData.Sample.WebApi.Infrastructure.Db;

using Microsoft.EntityFrameworkCore;

using OData.Sample.WebApi.Domain.Entities;

public partial class ODataSampleContext : DbContext
{
	public virtual DbSet<WorldRegion>? WorldRegions { get; set; }
	public virtual DbSet<CountryRegion>? CountryRegions { get; set; }
	public virtual DbSet<Country>? Countries { get; set; }

	public ODataSampleContext(DbContextOptions<ODataSampleContext> options)
		: base(options)
	{

	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		modelBuilder.ApplyConfiguration(new Configurations.WorldRegionConfiguration());
		modelBuilder.ApplyConfiguration(new Configurations.CountryRegionConfiguration());
		modelBuilder.ApplyConfiguration(new Configurations.CountryConfiguration());

		OnModelCreatingPartial(modelBuilder);
	}

	partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

namespace OData.Sample.WebApi.Infrastructure.Db;

using System.IO;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

public class ODataSampleContextDesignFactory : IDesignTimeDbContextFactory<ODataSampleContext>
{
	public ODataSampleContext CreateDbContext(string[] args)
	{
		var configuration = new ConfigurationBuilder()
			.SetBasePath(Directory.GetCurrentDirectory())
			.AddJsonFile("migrationsettings.json")
			.Build();

		var contextBuilder = new DbContextOptionsBuilder();

		var connectionString = configuration["ConnectionString"];

		var optionsBuilder = new DbContextOptionsBuilder<ODataSampleContext>()
			.UseSqlite(connectionString);

		return new ODataSampleContext(optionsBuilder.Options);
	}
}

#pragma warning disable IDISP001 // Dispose created
namespace OData.Sample.WebApi.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using OData.Sample.WebApi.Infrastructure.Db;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddCustomCors(this IServiceCollection services)
	{
		services.AddCors(options =>
		{
			options.AddDefaultPolicy(
				builder =>
				{
					builder
					.AllowAnyOrigin()
					.AllowAnyMethod()
					.AllowAnyHeader();
				});
		});

		return services;
	}

	public static IServiceCollection AddCustomDbContext(this IServiceCollection services)
	{
		var provider = services.BuildServiceProvider();
		var configuration = provider.GetService<IConfiguration>();
		var section = configuration!.GetSection("DbSettings");
		var settings = section.Get<DbSettings>();

		if (settings is not null)
		{
			services.AddDbContext<ODataSampleContext>(options =>
			{
				options.UseSqlite(settings.ConnectionString!);
			});
		}

		return services;
	}
}

namespace OData.Sample.WebApi;

using System.Text.Json.Serialization;
using System.Text.Json;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.OData;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OData.Sample.WebApi.Infrastructure.Db.SeedServices.Abstract;

using OData.Sample.WebApi.Infrastructure.Db.SeedServices;

using OData.Sample.WebApi.Infrastructure.Extensions;
using OData.Sample.WebApi.Infrastructure.EdmModel;
using Microsoft.AspNetCore.OData.Query.Expressions;
using Serilog;
using Microsoft.Extensions.Hosting;

public class Startup
{
	public Startup(IConfiguration configuration) =>
		Configuration = configuration;

	public IConfiguration Configuration { get; }

	public void ConfigureServices(IServiceCollection services)
	{
		services.AddRouting(options => options.LowercaseUrls = true);
		services.AddLogging();
		services.AddEndpointsApiExplorer();
		services.AddSwaggerGen();
		services.AddCors();
		services.AddCustomDbContext();
		services.AddScoped<IODataSampleContextSeed, ODataSampleContextSeed>();

		// Controller
		services.AddControllers()
			.AddOData(options =>
			{
				options.AddRouteComponents("odata/v1", new CountriesEdmModel().GetEdmModel(),
					services => services.AddSingleton<ISearchBinder, CountrySearchBinder>());

				options.EnableQueryFeatures();
			})
			.AddJsonOptions(opts =>
			{
				opts.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
				opts.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
				opts.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
				opts.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
				opts.JsonSerializerOptions.MaxDepth = 10;
				opts.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
				opts.JsonSerializerOptions.WriteIndented = true;
				opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
			});
	}

	public void Configure(WebApplication app, IWebHostEnvironment env)
	{
		if (env.IsDevelopment())
		{
			app.UseSwagger();
			app.UseSwaggerUI();
		}

		// Use odata route debug, /$odata
		app.UseODataRouteDebug();

		app.UseSerilogRequestLogging();
		app.UseRouting();
		app.MapControllers();
		app.UseHttpsRedirection();
	}
}

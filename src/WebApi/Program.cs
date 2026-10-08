namespace OData.Sample.WebApi;

using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using OData.Sample.WebApi.Infrastructure.Db;
using OData.Sample.WebApi.Infrastructure.Db.SeedServices.Abstract;
using OData.Sample.WebApi.Infrastructure.Logging;

using Serilog;
using Serilog.Events;
using Serilog.Extensions.Logging;
using Serilog.Sinks.SystemConsole.Themes;

internal class Program
{
	private static async Task Main(string[] args)
	{
		Log.Logger = new LoggerConfiguration()
			 .MinimumLevel.Debug()
			 .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
			 .Enrich.FromLogContext()
			 .WriteTo.Console(theme: AnsiConsoleTheme.Code)
			 .CreateLogger();

		using var serilogFactory = new SerilogLoggerFactory(Log.Logger, dispose: true);
		var logger = serilogFactory.CreateLogger<Program>();

		try
		{
			CommonLogger.LogInformation(logger,
				$"Starting Web Host {typeof(Program).Assembly.GetName()}");

			var builder = CreateWebApplicationBuilder(args);
			var startup = new Startup(builder.Configuration);

			startup.ConfigureServices(builder.Services);

			using var app = builder.Build();
			startup.Configure(app, builder.Environment); // calling Configure method

			await app.MigrateDatabase<ODataSampleContext>(
				(context, services) =>
				{
					services.GetService<IODataSampleContextSeed>()!.Seed();
				})
				.RunAsync();
		}
		catch (Exception ex)
		{
			CommonLogger.LogCritical(logger, "Host terminated unexpectedly", ex: ex);
			throw;
		}
		finally
		{
			Log.CloseAndFlush();
		}
	}
	private static WebApplicationBuilder CreateWebApplicationBuilder(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);
		builder.Host
			.UseSerilog()
			.ConfigureAppConfiguration((hostingContext, config) =>
			{
				config.AddEnvironmentVariables();
				config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: false);
				config.AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json",
					optional: true, reloadOnChange: false);
			});

		return builder;
	}
}

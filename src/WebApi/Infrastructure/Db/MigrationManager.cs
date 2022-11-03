namespace OData.Sample.WebApi.Infrastructure.Db;

using System;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Polly;

public static class MigrationManager
{
	public static IHost MigrateDatabase<TContext>(
		this IHost host,
		Action<TContext, IServiceProvider> seeder)
		where TContext : DbContext
	{
		if (host == null)
		{
			throw new ArgumentNullException(nameof(host));
		}

		if (seeder == null)
		{
			throw new ArgumentNullException(nameof(seeder));
		}

		using (var scope = host.Services.CreateScope())
		{
			var services = scope.ServiceProvider;
			var logger = services.GetRequiredService<ILogger<TContext>>();
			var context = services.GetService<TContext>();
			var env = services.GetService<IHostEnvironment>();
			var config = services.GetService<IConfiguration>();

			var settings = config!.GetSection("DbSettings").Get<DbSettings>();

			if (context is not null)
			{
				if (settings.DoMigrations)
				{
					Migrate(context, logger);
				}

				if (settings.DoSeeding)
				{
					seeder(context, services);
				}
			}
		}

		return host;
	}

	private static void Migrate<TContext>(
		TContext context,
		ILogger logger)
	   where TContext : DbContext
	{
		try
		{
			const int Retries = 10;
			var retry = Policy.Handle<SqliteException>()
				.WaitAndRetry(
					retryCount: Retries,
					sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
					onRetry: (exception, _, retry, _) =>
						logger.LogError($"[{nameof(TContext)}] Exception {exception.GetType().Name} with message {exception.Message} detected on attempt {retry} of {Retries}", exception));

			context.Database.Migrate();
		}
		catch (Exception ex)
		{
			// Log errors or do anything you think it's needed
			logger.LogError($"An error occurred while migrating the database used on context {typeof(TContext).Name}", ex);
			throw; // rethrow as we rely on re-run policies
		}
	}
}

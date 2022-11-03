namespace OData.Sample.WebApi.Infrastructure.Db.SeedServices;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using OData.Sample.WebApi.Domain.Entities;
using OData.Sample.WebApi.Infrastructure.Db.SeedServices.Abstract;

public class ODataSampleContextSeed :
	SeedService,
	IODataSampleContextSeed
{
	private readonly ILogger<ODataSampleContextSeed> _logger;

	public ODataSampleContextSeed(
	ODataSampleContext context,
	ILogger<ODataSampleContextSeed> logger)
		: base(context) => _logger = logger ?? throw new ArgumentNullException(nameof(logger));

	public void Seed()
	{
		_logger.LogInformation("Clear old sample data");

		_ = Context.Database.ExecuteSqlRaw("delete from Countries");
		_ = Context.Database.ExecuteSqlRaw("delete from sqlite_sequence where name='Countries'");

		_ = Context.Database.ExecuteSqlRaw("delete from CountryRegions");
		_ = Context.Database.ExecuteSqlRaw("delete from sqlite_sequence where name='CountryRegions'");

		_ = Context.Database.ExecuteSqlRaw("delete from WorldRegions");
		_ = Context.Database.ExecuteSqlRaw("delete from sqlite_sequence where name='WorldRegions'");

		_logger.LogInformation("Seed WorldRegions");
		var jsonWorldRegions = ExtractJsonFileResource("WorldRegions.json");
		var dataWorldRegions = JsonConvert.DeserializeObject<List<WorldRegion>>(jsonWorldRegions);
		if (dataWorldRegions is not null)
		{
			Context.WorldRegions!.AddRange(dataWorldRegions);
			_ = Context.SaveChanges();
		}

		_logger.LogInformation("Seed CountryRegions");
		var jsonCountryRegions = ExtractJsonFileResource("CountryRegions.json");
		var dataCountryRegions = JsonConvert.DeserializeObject<List<CountryRegion>>(jsonCountryRegions);
		if (dataCountryRegions is not null)
		{
			Context.CountryRegions!.AddRange(dataCountryRegions);
			_ = Context.SaveChanges();
		}

		_logger.LogInformation("Seed Countries");
		var jsonCountries = ExtractJsonFileResource("Countries.json");
		var dataCountries = JsonConvert.DeserializeObject<List<Country>>(jsonCountries);
		if (dataCountries is not null)
		{
			Context.Countries!.AddRange(dataCountries);
			_ = Context.SaveChanges();
		}
	}

	private static string ExtractJsonFileResource(string name)
	{
		var ret = string.Empty;
		var a = System.Reflection.Assembly.GetExecutingAssembly();
		name = a.GetManifestResourceNames().Single(str => Regex.IsMatch(str, @$"\.{name}$", RegexOptions.ExplicitCapture));
		using (var resFilestream = a.GetManifestResourceStream(name))
		{
			if (resFilestream is not null)
			{
				using var reader = new StreamReader(resFilestream, Encoding.UTF8);
				ret = reader.ReadToEnd();
			}
		}
		return ret;
	}
}


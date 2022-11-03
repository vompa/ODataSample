namespace OData.Sample.Client.Commands;

using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.OData.Client;

using OData.Sample.Client.Commands.Abstract;
using OData.Sample.Client.Extensions;
using OData.Sample.WebApi;

using Spectre.Console;

internal class GetCountryCountByRegionCommand : IOdataCommand
{
	public string CommandText => "Get countries grouped";

	public void Execute(Uri serviceRoot)
	{
		AnsiConsole.Console.WriteLogMessage(CommandText);
		AnsiConsole.WriteLine();
		try
		{
			var query = new CountriesContext(serviceRoot).Countries
				.Where(e => new string[] { "AF", "EU", "SA", "EU", "OC" }.Contains(e.WorldRegion.ISO))
				.GroupBy(
					d1 => d1.WorldRegion.NameGER, (d2, d3) =>
					new
					{
						WorldRegion = d2,
						CountryCount = d3.Count()
					});

			var oDataQuery = (DataServiceQuery)query;

			var result = query
				.ToList()
				.Select(e => (Region: e.WorldRegion, Count: e.CountryCount))
				.OrderBy(e => e.Region);

			PrintRegionBar(result);

			AnsiConsole.Console.WriteLogMessage(oDataQuery.RequestUri.ToString(),
				"https://learn.microsoft.com/en-us/odata/client/grouping-and-aggregation");
		}
		catch (Exception ex)
		{
			AnsiConsole.WriteException(ex);
		}
	}

	private static void PrintRegionBar(IEnumerable<(string Region, int Count)> regions)
	{
		// grouped result
		var bar = new BarChart()
			.Width(60)
			.CenterLabel()
;
		foreach (var (region, count) in regions)
		{
			_ = bar.AddItem(region, count, Color.FromInt32(new Random().Next(100, 200)));
		}

		AnsiConsole.Write(bar);
		AnsiConsole.WriteLine();
	}

}

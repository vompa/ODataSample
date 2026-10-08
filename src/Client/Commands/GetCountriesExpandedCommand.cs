namespace OData.Sample.Client.Commands;

using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.OData.Client;

using OData.Sample.Client.Commands.Abstract;
using OData.Sample.Client.Extensions;
using OData.Sample.WebApi;

using Spectre.Console;

internal class GetCountriesExpandedCommand : IOdataCommand
{
	public string CommandText => "Get countries expanded regions";

	public void Execute(Uri serviceRoot)
	{
		AnsiConsole.Console.WriteLogMessage(CommandText);
		try
		{
			var query = new CountriesContext(serviceRoot).Countries
				.Expand(e => e.CountryRegion)
				.Expand(e => e.WorldRegion)
				.Expand("CountryRegion($expand=WorldRegion)") // expand nested max deep = 2
				.Where(e => new string[] { "AT", "DE", "IT" }.Contains(e.ISO2))
				.OrderBy(e => e.DisplayNameGER);

			var oDataQuery = (DataServiceQuery)query;
			var result = query.ToList();

			AnsiConsole.Console.WriteJson(result);

			PrintExpanded(result);

			AnsiConsole.Console
				.WriteLogMessage(oDataQuery.RequestUri.ToString(), "https://learn.microsoft.com/en-us/odata/client/getting-started");
		}
		catch (Exception ex)
		{
			AnsiConsole.WriteException(ex);
		}
	}

	private static void PrintExpanded(IEnumerable<Country> laender)
	{
		var empty = "--";
		var table = new Table();
		table.Border(TableBorder.Rounded);
		table.AddColumn("ISO2");
		table.AddColumn("CountryRegion");
		table.AddColumn("WorldRegion");
		table.AddColumn("DisplayNameGER");
		foreach (var land in laender)
		{
			table.AddRow(
				land.ISO2 ?? empty,
				land.CountryRegion?.NameGER ?? empty,
				land.WorldRegion?.NameGER ?? empty,
				land.DisplayNameGER ?? empty);
		}
		AnsiConsole.Write(table);
		AnsiConsole.WriteLine();
	}
}

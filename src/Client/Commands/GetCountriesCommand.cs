namespace OData.Sample.Client.Commands;

using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.OData.Client;

using OData.Sample.Client.Commands.Abstract;
using OData.Sample.Client.Extensions;
using OData.Sample.WebApi;

using Spectre.Console;

internal class GetCountriesCommand : IOdataCommand
{
	public string CommandText => "Get countries";

	public void Execute(Uri serviceRoot)
	{
		AnsiConsole.Console.WriteLogMessage(CommandText);
		try
		{
			var query = new CountriesContext(serviceRoot).Countries
				.Where(e => new string[] { "AT", "DE", "IT" }.Contains(e.ISO2))
				.OrderBy(e => e.ISO3);

			var oDataQuery = (DataServiceQuery)query;
			var result = query.ToList();

			AnsiConsole.Console.WriteJson(result);

			Print(result);

			AnsiConsole.Console
				.WriteLogMessage(oDataQuery.RequestUri.ToString(), "https://learn.microsoft.com/en-us/odata/client/getting-started");
		}
		catch (Exception ex)
		{
			AnsiConsole.WriteException(ex);
		}
	}

	private static void Print(IEnumerable<Country> laender)
	{
		var empty = "--";
		var table = new Table();
		table.Border(TableBorder.Rounded);
		table.AddColumn("Id");
		table.AddColumn("DisplayName");
		table.AddColumn("EU");
		table.AddColumn("EWR");
		table.AddColumn("ISO2");
		table.AddColumn("ISO3");
		table.AddColumn("Name");
		foreach (var land in laender)
		{
			table.AddRow(
				land.Id.ToString(),
				land.DisplayName ?? empty,
				land.EU ?? empty,
				land.EWR ?? empty,
				land.ISO2 ?? empty,
				land.ISO3 ?? empty,
				land.Name ?? empty);
		}
		AnsiConsole.Write(table);
		AnsiConsole.WriteLine();
	}

}

namespace OData.Sample.Client.Commands;

using System;
using System.Linq;

using OData.Sample.Client.Commands.Abstract;
using OData.Sample.Client.Extensions;
using OData.Sample.WebApi;

using Spectre.Console;

internal class DeleteCountryCommand : IOdataCommand
{
	public string CommandText => "Delete country in Afrika";

	public void Execute(Uri serviceRoot)
	{
		AnsiConsole.Console.WriteLogMessage(CommandText);
		try
		{
			var context = new CountriesContext(serviceRoot);
			var countryToDelete = context.Countries.
				Where(e => e.WorldRegionId == 903)
				.FirstOrDefault();

			context.DeleteObject(countryToDelete);
			var responses = context.SaveChanges();

			AnsiConsole.Console
				.WriteDataServiceResponse<Country>(responses,
					Extensions.AnsiConsoleExtensions.SaveOperation.DeleteEntity);
		}
		catch (Exception ex)
		{
			AnsiConsole.WriteException(ex);
		}
	}
}

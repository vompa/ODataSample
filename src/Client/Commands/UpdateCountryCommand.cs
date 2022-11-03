namespace OData.Sample.Client.Commands;

using System;
using System.Linq;

using Bogus;

using OData.Sample.Client.Commands.Abstract;
using OData.Sample.Client.Extensions;
using OData.Sample.WebApi;

using Spectre.Console;

internal class UpdateCountryCommand : IOdataCommand
{
	public string CommandText => "Update country AT";

	public void Execute(Uri serviceRoot)
	{
		AnsiConsole.Console.WriteLogMessage(CommandText);
		try
		{
			var context = new CountriesContext(serviceRoot);
			var countryToChange = context.Countries
				.Where(e => e.Id == 40)
				.Single();

			countryToChange.DisplayName = new Faker("de_AT").Address.Country();
			context.UpdateObject(countryToChange);

			var responses = context.SaveChanges();

			AnsiConsole.Console
				.WriteDataServiceResponse<Country>(responses,
					Extensions.AnsiConsoleExtensions.SaveOperation.UpdateEntity);
		}
		catch (Exception ex)
		{
			AnsiConsole.WriteException(ex);
		}
	}
}

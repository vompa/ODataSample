namespace OData.Sample.Client.Commands;

using System;

using Bogus;
using Bogus.DataSets;

using OData.Sample.Client.Commands.Abstract;
using OData.Sample.Client.Extensions;
using OData.Sample.WebApi;

using Spectre.Console;

internal class AddCountryCommand : IOdataCommand
{
	public string CommandText => "Add new country";

	public void Execute(Uri serviceRoot)
	{
		AnsiConsole.Console.WriteLogMessage(CommandText);
		try
		{
			var context = new CountriesContext(serviceRoot);

			var country = new CountryFaker()
				.Generate();

			country.Id = 0;
			country.CountryRegionId = 926;
			country.WorldRegionId = 908;

			context.AddToCountries(country);

			var responses = context.SaveChanges();

			AnsiConsole.Console
				.WriteDataServiceResponse<Country>(responses,
					Extensions.AnsiConsoleExtensions.SaveOperation.AddEntity);
		}
		catch (Exception ex)
		{
			AnsiConsole.WriteException(ex);
		}
	}

	public class CountryFaker : Faker<Country>
	{
		public CountryFaker()
		{
			Ignore(e => e.Id);
			Ignore(e => e.WorldRegion);
			Ignore(e => e.CountryRegion);
			RuleFor(e => e.DisplayName, f => f.Address.Country());
			RuleFor(e => e.DisplayNameGER, f => f.Address.Country());
			RuleFor(e => e.EU, f => "N");
			RuleFor(e => e.EWR, f => "N");
			RuleFor(e => e.ISO2, f => f.Address.CountryCode(Iso3166Format.Alpha2));
			RuleFor(e => e.ISO3, f => f.Address.CountryCode(Iso3166Format.Alpha3));
			RuleFor(e => e.Name, f => f.Address.Country());
			RuleFor(e => e.NameGER, f => f.Address.Country());
		}
	}
}

namespace OData.Sample.WebApi.Infrastructure.EdmModel;

using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;

using OData.Sample.WebApi.Domain.Entities;

public class CountriesEdmModel
{
	///<Summary>
	///<see href="https://learn.microsoft.com/en-us/odata/webapi/convention-model-builder">learn.microsoft.com</see>
	///</Summary>
	public IEdmModel GetEdmModel()
	{
		ODataConventionModelBuilder builder = new()
		{
			Namespace = "OData.Sample.WebApi",
			ContainerName = "CountriesContext",
			BindingOptions = NavigationPropertyBindingOption.Auto
		};

		builder.EntitySet<WorldRegion>("WorldRegions")
			.EntityType
			.HasKey(table => table.Id);

		builder.EntitySet<CountryRegion>("CountryRegions")
			.EntityType
			.HasKey(table => table.Id);

		builder.EntitySet<Country>("Countries")
			.EntityType
			.HasKey(table => table.Id);

		return builder.GetEdmModel();
	}
}

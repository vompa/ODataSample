namespace OData.Sample.WebApi.Domain.Entities;

public partial class Country : BaseEntity
{
	public string? DisplayName { get; set; }
	public string? DisplayNameGER { get; set; }
	public string? EU { get; set; }
	public string? EWR { get; set; }
	public string? ISO2 { get; set; }
	public string? ISO3 { get; set; }
	public string? Name { get; set; }
	public string? NameGER { get; set; }

	public virtual CountryRegion? CountryRegion { get; set; }
	public int? CountryRegionId { get; set; }

	public virtual WorldRegion? WorldRegion { get; set; }
	public int? WorldRegionId { get; set; }
}

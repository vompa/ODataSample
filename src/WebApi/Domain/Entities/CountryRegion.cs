namespace OData.Sample.WebApi.Domain.Entities;

using System.Collections.Generic;

public partial class CountryRegion : BaseEntity
{
	public string? ISO { get; set; }
	public string? Name { get; set; }
	public string? NameGER { get; set; }

	public virtual WorldRegion? WorldRegion { get; set; }
	public int? WorldRegionId { get; set; }

	public virtual ICollection<Country>? Countries { get; set; }
}

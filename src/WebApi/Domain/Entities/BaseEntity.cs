namespace OData.Sample.WebApi.Domain.Entities;

using OData.Sample.WebApi.Domain.Entities.Abstract;

public abstract class BaseEntity : IBaseEntity
{
	public int Id { get; set; }
}

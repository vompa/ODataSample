namespace OData.Sample.WebApi.Infrastructure.Db.SeedServices;

using System;

public abstract class SeedService
{
	protected SeedService(ODataSampleContext context)
		=> Context = context ?? throw new ArgumentNullException(nameof(context));

	protected ODataSampleContext Context { get; }
}

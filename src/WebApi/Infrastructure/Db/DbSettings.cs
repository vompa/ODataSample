namespace OData.Sample.WebApi.Infrastructure.Db;

public class DbSettings
{
	public string? ConnectionString { get; set; }

	public bool DoMigrations { get; set; }

	public bool DoSeeding { get; set; }
}

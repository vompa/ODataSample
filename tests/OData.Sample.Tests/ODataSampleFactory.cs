using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OData.Sample.WebApi;
using OData.Sample.WebApi.Infrastructure.Db;

namespace OData.Sample.Tests;

/// <summary>
/// Startet die echte Anwendung im Speicher mit einer eigenen SQLite-Datei.
/// Das Repository enthaelt bewusst keine EF-Migration (siehe README). Fuer die Tests wird das Schema deshalb vorab
/// aus dem Modell erzeugt; Migrate() im Anwendungsstart findet dann nichts mehr zu tun, das Seeding laeuft normal.
/// </summary>
public sealed class ODataSampleFactory : WebApplicationFactory<Startup>
{
    private readonly string _dbPath = Path.Combine(Path.GetTempPath(), "odata-sample-" + Guid.NewGuid().ToString("N") + ".db");

    public ODataSampleFactory()
    {
        var options = new DbContextOptionsBuilder<ODataSampleContext>().UseSqlite("Data Source=" + _dbPath).Options;
        using var context = new ODataSampleContext(options);
        context.Database.EnsureCreated();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        // Die Anwendung laedt appsettings.json NACH allen anderen Konfigurationsquellen. Eine per UseSetting gesetzte
        // Verbindungszeichenfolge wuerde deshalb ueberschrieben. Darum wird die DbContext-Registrierung ersetzt.
        builder.ConfigureTestServices(services =>
        {
            foreach (var descriptor in services.Where(d => d.ServiceType == typeof(DbContextOptions<ODataSampleContext>)).ToList())
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<ODataSampleContext>(options => options.UseSqlite("Data Source=" + _dbPath));
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
        {
            SqliteConnection.ClearAllPools();
            foreach (var file in new[] { _dbPath, _dbPath + "-wal", _dbPath + "-shm" })
            {
                File.Delete(file);
            }
        }
    }
}

using System.Globalization;
using System.Net;
using System.Text;
using System.Text.Json;

namespace OData.Sample.Tests;

/// <summary>Schreibende Operationen. Jeder Test legt sein eigenes Land an und räumt wieder auf.</summary>
public sealed class ODataCrudTests : IClassFixture<ODataSampleFactory>
{
    private readonly HttpClient _client;

    public ODataCrudTests(ODataSampleFactory factory) => _client = factory.CreateClient();

    private static StringContent Json(string body) => new StringContent(body, Encoding.UTF8, "application/json");

    private static Uri Url(string path) => new Uri(path, UriKind.Relative);

    private async Task<int> CountAsync() =>
        int.Parse(await _client.GetStringAsync(Url("/odata/v1/Countries/$count")), CultureInfo.InvariantCulture);

    private async Task<int> CreateCountryAsync(string iso2)
    {
        var body = "{\"Name\":\"Testland\",\"NameGER\":\"Testland\",\"DisplayName\":\"Testland\",\"DisplayNameGER\":\"Testland\","
                   + "\"ISO2\":\"" + iso2 + "\",\"ISO3\":\"" + iso2 + "X\",\"WorldRegionId\":908}";
        var response = await _client.PostAsync(Url("/odata/v1/Countries"), Json(body));
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var created = JsonDocument.Parse(await response.Content.ReadAsStringAsync()).RootElement;
        return created.GetProperty("Id").GetInt32();
    }

    private async Task DeleteCountryAsync(int id)
    {
        var response = await _client.DeleteAsync(Url("/odata/v1/Countries(" + id + ")"));
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Post_creates_a_country_that_can_be_read_back_and_deleted()
    {
        var id = await CreateCountryAsync("ZA");

        var read = await _client.GetAsync(Url("/odata/v1/Countries(" + id + ")?$select=ISO2,Name"));
        Assert.Equal(HttpStatusCode.OK, read.StatusCode);
        Assert.Contains("Testland", await read.Content.ReadAsStringAsync(), StringComparison.Ordinal);

        await DeleteCountryAsync(id);
        Assert.Equal(HttpStatusCode.NotFound, (await _client.GetAsync(Url("/odata/v1/Countries(" + id + ")"))).StatusCode);
    }

    [Fact]
    public async Task Patch_changes_only_the_given_property()
    {
        var id = await CreateCountryAsync("ZB");

        var patch = await _client.PatchAsync(Url("/odata/v1/Countries(" + id + ")"), Json("{\"DisplayNameGER\":\"Neuer Name\"}"));
        var read = JsonDocument.Parse(await _client.GetStringAsync(Url("/odata/v1/Countries(" + id + ")"))).RootElement;

        Assert.True(patch.IsSuccessStatusCode, patch.StatusCode.ToString());
        Assert.Equal("Neuer Name", read.GetProperty("DisplayNameGER").GetString());
        Assert.Equal("Testland", read.GetProperty("DisplayName").GetString());

        await DeleteCountryAsync(id);
    }

    [Fact]
    public async Task Patch_of_an_unknown_country_returns_not_found()
    {
        var response = await _client.PatchAsync(Url("/odata/v1/Countries(999999)"), Json("{\"DisplayNameGER\":\"x\"}"));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_of_an_unknown_country_returns_not_found()
    {
        var response = await _client.DeleteAsync(Url("/odata/v1/Countries(999999)"));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Created_country_is_counted_and_disappears_after_delete()
    {
        var before = await CountAsync();
        var id = await CreateCountryAsync("ZC");
        var during = await CountAsync();
        await DeleteCountryAsync(id);
        var after = await CountAsync();

        Assert.Equal(before + 1, during);
        Assert.Equal(before, after);
    }
}

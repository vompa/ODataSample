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
        try
        {
            var read = await _client.GetAsync(Url("/odata/v1/Countries(" + id + ")?$select=ISO2,Name"));
            var json = JsonDocument.Parse(await read.Content.ReadAsStringAsync()).RootElement;

            Assert.Equal(HttpStatusCode.OK, read.StatusCode);
            Assert.Equal("ZA", json.GetProperty("ISO2").GetString());
            Assert.Equal("Testland", json.GetProperty("Name").GetString());
        }
        finally
        {
            await DeleteCountryAsync(id);
        }

        Assert.Equal(HttpStatusCode.NotFound, (await _client.GetAsync(Url("/odata/v1/Countries(" + id + ")"))).StatusCode);
    }

    [Fact]
    public async Task Patch_changes_only_the_given_property()
    {
        var id = await CreateCountryAsync("ZB");
        try
        {
            var patch = await _client.PatchAsync(Url("/odata/v1/Countries(" + id + ")"), Json("{\"DisplayNameGER\":\"Neuer Name\"}"));
            var read = JsonDocument.Parse(await _client.GetStringAsync(Url("/odata/v1/Countries(" + id + ")"))).RootElement;

            Assert.True(patch.IsSuccessStatusCode, patch.StatusCode.ToString());
            Assert.Equal("Neuer Name", read.GetProperty("DisplayNameGER").GetString());
            Assert.Equal("Testland", read.GetProperty("DisplayName").GetString());
        }
        finally
        {
            await DeleteCountryAsync(id);
        }
    }

    [Theory]
    [InlineData("PATCH")]
    [InlineData("DELETE")]
    [InlineData("GET")]
    public async Task Operations_on_an_unknown_country_return_not_found(string method)
    {
        using var request = new HttpRequestMessage(new HttpMethod(method), Url("/odata/v1/Countries(999999)"));
        if (method == "PATCH")
        {
            request.Content = Json("{\"DisplayNameGER\":\"x\"}");
        }

        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Theory]
    [InlineData("{bad json")]
    [InlineData("{\"Name\":")]
    public async Task Post_with_malformed_json_returns_bad_request_and_creates_nothing(string body)
    {
        var before = await CountAsync();

        var response = await _client.PostAsync(Url("/odata/v1/Countries"), Json(body));
        var error = JsonDocument.Parse(await response.Content.ReadAsStringAsync()).RootElement.GetProperty("error");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(JsonValueKind.String, error.GetProperty("code").ValueKind);
        Assert.Equal(before, await CountAsync());
    }

    [Theory]
    [InlineData("{\"Name\":\"x\"}", "DisplayName")]
    [InlineData("{\"Name\":\"x\",\"NameGER\":\"x\",\"DisplayName\":\"   \",\"DisplayNameGER\":\"x\"}", "DisplayName")]
    [InlineData("{}", "NameGER")]
    public async Task Post_with_missing_required_field_returns_bad_request_and_creates_nothing(string body, string missingField)
    {
        var before = await CountAsync();

        var response = await _client.PostAsync(Url("/odata/v1/Countries"), Json(body));
        var error = JsonDocument.Parse(await response.Content.ReadAsStringAsync()).RootElement.GetProperty("error");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains(missingField, error.GetRawText());
        Assert.Equal(before, await CountAsync());
    }

    [Theory]
    [InlineData("/odata/v1/CountryRegions")]
    [InlineData("/odata/v1/WorldRegions")]
    public async Task Post_with_empty_body_to_region_collections_returns_bad_request(string path)
    {
        var response = await _client.PostAsync(Url(path), Json("{}"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Created_country_is_counted_and_disappears_after_delete()
    {
        var before = await CountAsync();
        var id = await CreateCountryAsync("ZC");
        int during;
        try
        {
            during = await CountAsync();
        }
        finally
        {
            await DeleteCountryAsync(id);
        }

        Assert.Equal(before + 1, during);
        Assert.Equal(before, await CountAsync());
    }
}

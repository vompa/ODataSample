using System.Net;
using System.Text.Json;

namespace OData.Sample.Tests;

/// <summary>Lesende OData-Abfragen gegen den echten Server mit den eingebetteten Seed-Daten (264 Länder).</summary>
public sealed class ODataQueryTests : IClassFixture<ODataSampleFactory>
{
    private readonly HttpClient _client;

    public ODataQueryTests(ODataSampleFactory factory) => _client = factory.CreateClient();

    private async Task<JsonElement> GetJsonAsync(string url)
    {
        var response = await _client.GetAsync(new Uri(url, UriKind.Relative));
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        return JsonDocument.Parse(await response.Content.ReadAsStringAsync()).RootElement;
    }

    [Theory]
    [InlineData("/odata/v1/Countries/$count", "264")]
    [InlineData("/odata/v1/CountryRegions/$count", "22")]
    [InlineData("/odata/v1/WorldRegions/$count", "6")]
    public async Task Entity_sets_are_seeded_from_the_embedded_json(string url, string expected)
    {
        var body = await _client.GetStringAsync(new Uri(url, UriKind.Relative));

        Assert.Equal(expected, body);
    }

    [Fact]
    public async Task Metadata_exposes_the_three_entity_sets()
    {
        var xml = await _client.GetStringAsync(new Uri("/odata/v1/$metadata", UriKind.Relative));

        Assert.Contains("EntitySet Name=\"Countries\"", xml, StringComparison.Ordinal);
        Assert.Contains("EntitySet Name=\"CountryRegions\"", xml, StringComparison.Ordinal);
        Assert.Contains("EntitySet Name=\"WorldRegions\"", xml, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Filter_select_and_orderby_work_together()
    {
        var json = await GetJsonAsync("/odata/v1/Countries?$filter=ISO2 in ('AT','DE')&$select=ISO2,ISO3&$orderby=ISO3");

        var items = json.GetProperty("value");
        Assert.Equal(2, items.GetArrayLength());
        Assert.Equal("AUT", items[0].GetProperty("ISO3").GetString());
        Assert.Equal("DEU", items[1].GetProperty("ISO3").GetString());
    }

    [Fact]
    public async Task Entity_can_be_read_by_key()
    {
        var found = await GetJsonAsync("/odata/v1/Countries?$filter=ISO2 eq 'AT'&$select=Id");
        var id = found.GetProperty("value")[0].GetProperty("Id").GetInt32();

        var byKey = await GetJsonAsync("/odata/v1/Countries(" + id + ")?$select=ISO2,Name");

        Assert.Equal("AT", byKey.GetProperty("ISO2").GetString());
        Assert.Equal("Austria", byKey.GetProperty("Name").GetString());
    }

    [Fact]
    public async Task Unknown_key_returns_not_found()
    {
        var response = await _client.GetAsync(new Uri("/odata/v1/Countries(999999)", UriKind.Relative));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Expand_resolves_navigation_properties_including_a_nested_expand()
    {
        var json = await GetJsonAsync(
            "/odata/v1/Countries?$filter=ISO2 eq 'AT'&$select=ISO2&$expand=WorldRegion($select=Name),CountryRegion($select=Name;$expand=WorldRegion($select=Name))");

        var austria = json.GetProperty("value")[0];
        Assert.Equal("Europe", austria.GetProperty("WorldRegion").GetProperty("Name").GetString());
        Assert.Equal("Western Europe", austria.GetProperty("CountryRegion").GetProperty("Name").GetString());
        Assert.Equal("Europe", austria.GetProperty("CountryRegion").GetProperty("WorldRegion").GetProperty("Name").GetString());
    }

    [Fact]
    public async Task Count_option_and_paging_are_supported()
    {
        var json = await GetJsonAsync("/odata/v1/Countries?$count=true&$top=3&$skip=2&$orderby=Id");

        Assert.Equal(264, json.GetProperty("@odata.count").GetInt32());
        Assert.Equal(3, json.GetProperty("value").GetArrayLength());
    }

    [Fact]
    public async Task Custom_search_maps_the_german_term_to_the_world_region_europe()
    {
        // Referenz: derselbe Zuschnitt über $filter (WorldRegionId 908 = Europa)
        var viaSearch = await GetJsonAsync("/odata/v1/Countries?$search=LaenderInEuropa&$count=true&$top=1");
        var viaFilter = await GetJsonAsync("/odata/v1/Countries?$filter=WorldRegionId eq 908&$count=true&$top=1");

        var europe = viaSearch.GetProperty("@odata.count").GetInt32();
        Assert.Equal(viaFilter.GetProperty("@odata.count").GetInt32(), europe);
        Assert.InRange(europe, 40, 70);
    }

    [Fact]
    public async Task Unknown_search_term_is_rejected()
    {
        var response = await _client.GetAsync(new Uri("/odata/v1/Countries?$search=Gibtsnicht", UriKind.Relative));

        Assert.False(response.IsSuccessStatusCode);
    }

    [Fact]
    public async Task Apply_groupby_counts_the_countries_per_world_region()
    {
        var json = await GetJsonAsync("/odata/v1/Countries?$apply=groupby((WorldRegionId),aggregate($count as CountryCount))");

        var groups = json.GetProperty("value").EnumerateArray().ToList();
        Assert.Equal(6, groups.Count);
        Assert.Equal(264, groups.Sum(g => g.GetProperty("CountryCount").GetInt32()));
    }

    [Fact]
    public async Task Country_regions_are_paged_with_a_server_page_size_of_ten()
    {
        var json = await GetJsonAsync("/odata/v1/CountryRegions");

        Assert.Equal(10, json.GetProperty("value").GetArrayLength());
        Assert.True(json.TryGetProperty("@odata.nextLink", out _));
    }

    [Fact]
    public async Task Plain_controller_returns_all_countries_as_a_json_array()
    {
        var json = await GetJsonAsync("/allelaender");

        Assert.Equal(JsonValueKind.Array, json.ValueKind);
        Assert.Equal(264, json.GetArrayLength());
    }
}

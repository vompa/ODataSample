using System.Globalization;
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
    [InlineData("/odata/v1/Countries/$count", 264)]
    [InlineData("/odata/v1/CountryRegions/$count", 22)]
    [InlineData("/odata/v1/WorldRegions/$count", 6)]
    public async Task Entity_sets_are_seeded_from_the_embedded_json(string path, int expected)
    {
        var body = await _client.GetStringAsync(new Uri(path, UriKind.Relative));

        Assert.Equal(expected, int.Parse(body, CultureInfo.InvariantCulture));
    }

    [Theory]
    [InlineData("contains(Name,'land')", 31)]
    [InlineData("startswith(Name,'Au')", 2)]
    [InlineData("ISO2 ne 'AT'", 263)]
    [InlineData("ISO2 eq 'AT' or ISO2 eq 'DE'", 2)]
    [InlineData("ISO2 eq 'XX'", 0)]
    public async Task Filter_expressions_return_the_expected_number_of_countries(string filter, int expected)
    {
        var json = await GetJsonAsync("/odata/v1/Countries?$count=true&$top=0&$filter=" + filter);

        Assert.Equal(expected, json.GetProperty("@odata.count").GetInt32());
        Assert.Equal(0, json.GetProperty("value").GetArrayLength());
    }

    [Fact]
    public async Task Top_zero_returns_no_items_but_the_full_count()
    {
        var json = await GetJsonAsync("/odata/v1/Countries?$count=true&$top=0");

        Assert.Equal(264, json.GetProperty("@odata.count").GetInt32());
        Assert.Equal(0, json.GetProperty("value").GetArrayLength());
    }

    [Fact]
    public async Task Orderby_desc_returns_the_last_iso2_code_first()
    {
        var json = await GetJsonAsync("/odata/v1/Countries?$orderby=ISO2 desc&$top=1&$select=ISO2");

        Assert.Equal("ZW", json.GetProperty("value")[0].GetProperty("ISO2").GetString());
    }

    [Theory]
    [InlineData("$filter=Foo eq 1", "Foo")]
    [InlineData("$filter=ISO2 eq", "Expression expected")]
    [InlineData("$top=-1", "non-negative integer")]
    [InlineData("$search=Gibtsnicht", "")]
    public async Task Invalid_query_options_are_rejected_with_a_bad_request_and_an_odata_error(string query, string messagePart)
    {
        var response = await _client.GetAsync(new Uri("/odata/v1/Countries?" + query, UriKind.Relative));
        var error = JsonDocument.Parse(await response.Content.ReadAsStringAsync()).RootElement.GetProperty("error");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        // Der Server liefert fuer error.code derzeit einen leeren String; die Eigenschaft muss aber vorhanden sein.
        Assert.Equal(JsonValueKind.String, error.GetProperty("code").ValueKind);
        Assert.Contains(messagePart, error.GetProperty("message").GetString(), StringComparison.Ordinal);
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

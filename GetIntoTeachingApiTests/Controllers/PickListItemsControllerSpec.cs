using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using GetIntoTeachingApiTests.Contracts;
using GetIntoTeachingApiTests.Helpers;
using Microsoft.PowerPlatform.Dataverse.Client.Extensions;
using Newtonsoft.Json;

[Collection("Database")]
public class PickListItemsControllerSpec : BaseTests
{
    private string _baseUrl = "/api/pick_list_items/";


    public PickListItemsControllerSpec(DatabaseFixture databaseFixture) : base(databaseFixture)
    {
    }

    [Theory]
    [MemberData(nameof(GetPickListData))]
    public async Task GetPickListItems_ReturnsOk(string entityName, string attributeName, IEnumerable<PickListItem> pickListExpected)
    {
        await SeedPickListItems();
        HttpResponseMessage response = await HttpClient.GetAsync($"{_baseUrl}{entityName}/{attributeName}");

        response.EnsureSuccessStatusCode();
        string content = await response.Content.ReadAsStringAsync();
        IEnumerable<PickListItem> picklistResponse = JsonConvert.DeserializeObject<IEnumerable<PickListItem>>(content);
        Assert.NotEmpty(picklistResponse);
        Assert.Equal(pickListExpected.Count(), picklistResponse.Count());
        Assert.Equivalent(pickListExpected, picklistResponse);
    }

    [Fact]
    public async Task CallToNonExistantController_Return404()
    {
        HttpResponseMessage response = await HttpClient.GetAsync($"{_baseUrl}not/a_picklist");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }


    public static IEnumerable<object[]> GetPickListData()
    {
        var basePath = Path.Combine(Directory.GetCurrentDirectory(), "Contracts/Data/pick_list_items");

        if (!Directory.Exists(basePath))
        {
            return Enumerable.Empty<object[]>();
        }

        var entityDirectories = Directory.EnumerateDirectories(basePath);

        return entityDirectories.SelectMany(entityDir =>
        {
            var entityName = new DirectoryInfo(entityDir).Name;
            return Directory.EnumerateFiles(entityDir, "*.json")
                .Select(file => new object[] { entityName, Path.GetFileNameWithoutExtension(file), GetPickList(entityName, Path.GetFileNameWithoutExtension(file)) });
        });
    }

    private static IEnumerable<PickListItem> GetPickList(string entityName, string attributeName)
    {
        string path = $"./Contracts/Data/pick_list_items/{entityName}/{attributeName}.json";
        string content = File.ReadAllText(path);
        return JsonConvert.DeserializeObject<IEnumerable<PickListItem>>(content);
    }
}

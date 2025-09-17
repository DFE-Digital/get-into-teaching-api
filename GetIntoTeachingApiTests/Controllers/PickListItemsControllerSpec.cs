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


    public PickListItemsControllerSpec(DatabaseFixture databaseFixture): base(databaseFixture)
    {
    }
    
    [Theory]
    [InlineData("candidate", "initial_teacher_training_years")]
    [InlineData("qualification", "types")]
    public async Task GetCandidateInitialTeacherTrainingYears_ReturnsOk(string entityName, string attributeName)
    {
        await SetupPickLists();
        HttpResponseMessage response = await HttpClient.GetAsync($"{_baseUrl}{entityName}/{attributeName}");
        
        response.EnsureSuccessStatusCode();
        string content = await response.Content.ReadAsStringAsync();
        var picklistResponse = JsonConvert.DeserializeObject<IEnumerable<PickListItem>>(content);
        var pickListExpected = GetPickList(entityName, attributeName);
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

    private IEnumerable<PickListItem> GetPickList(string entityName, string attributeName)
    {
        string path = $"./Contracts/Data/pick_list_items/{entityName}/{attributeName}.json";
        string content = File.ReadAllText(path);
        return JsonConvert.DeserializeObject<IEnumerable<PickListItem>>(content);
    }
}



using Benday.DemoApp.Api;
using Benday.DemoApp.WebApi;
using Benday.Identity.CosmosDb;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Text.Json;
using Xunit.Abstractions;

namespace Benday.DemoApp.IntegrationTests;

public class IdentityUserControllerFixture : TestClassBase
{
    public IdentityUserControllerFixture(ITestOutputHelper output) : base(output)
    {

    }
    
    [Fact]
    public async Task Index_Get_WithOwnerId_ReturnsSuccess()
    {
        // arrange
        using var factory = new WebApplicationFactory<MarkerClassForTesting>();

        var client = factory.CreateClient();

        var ownerId = "fakeownerid";

        // act
        var response = await client.GetAsync($"api/IdentityUser/{ownerId}");

        // assert
        Assert.NotNull(response);

        var content = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode == false)
        {

            WriteLine(content);
            TestUtilities.CheckForDependencyError(content);
            Assert.Fail();
        }
        else
        {
            WriteLine(content);

            var IdentityUsers = JsonSerializer.Deserialize<List<IdentityUser>>(content);

            Assert.True(response.IsSuccessStatusCode);
        }        
    }
}

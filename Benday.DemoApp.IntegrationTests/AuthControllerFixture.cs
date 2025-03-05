using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Benday.DemoApp.WebApi;
using Benday.DemoApp.WebApi.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit.Abstractions;

namespace Benday.DemoApp.IntegrationTests;

public class AuthControllerFixture : TestClassBase
{
    private const string PASSWORD = "password";

    public AuthControllerFixture(ITestOutputHelper output) : base(output)
    {

    }
    
    [Fact]
    public async Task Register_Post_NewUser()
    {
        // arrange
        using var factory = new WebApplicationFactory<MarkerClassForTesting>();

        var client = factory.CreateClient();

        var now = DateTime.Now.Ticks.ToString();
        
        var model = new RegisterModel()
        {
            Email = $"email_{now}@email.com",
            Password = PASSWORD
        };

        var postContent = new StringContent(
            JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");

        // act
        var response = await client.PostAsync("/api/auth/register", postContent);

        // assert
        var responseContent = await response.Content.ReadAsStringAsync();
        
        WriteLine(responseContent);

        response.EnsureSuccessStatusCode();

        Assert.Contains("User registered successfully!", responseContent);
    }
}

using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Benday.DemoApp.WebApi;
using Benday.DemoApp.WebApi.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit.Abstractions;

namespace Benday.DemoApp.IntegrationTests;

public class SecuredControllerFixture : TestClassBase
{
    private const string PASSWORD = "password";

    public SecuredControllerFixture(ITestOutputHelper output) : base(output)
    {

    }
    private async Task<LoginTokenResponse> Login(HttpClient client, RegisterModel user)
    {
        var postContent = new StringContent(
            JsonSerializer.Serialize(user), Encoding.UTF8, "application/json");

        // attempt to login
        var response = await client.PostAsync("/api/auth/login", postContent);

        var json = await response.Content.ReadAsStringAsync();

        json.Should().NotBeNullOrWhiteSpace();

        response.EnsureSuccessStatusCode();

        var token = JsonSerializer.Deserialize<LoginTokenResponse>(json);

        if (token == null)
        {
            throw new InvalidOperationException("Token was null.");
        }

        return token;
    }

    private async Task<RegisterModel> CreateUser()
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

        return model;
    }

    [Fact]
    public async Task Secured_Protected_SucceedsWhenLoggedIn()
    {
        // arrange
        var user = await CreateUser();
        
        using var factory = new WebApplicationFactory<MarkerClassForTesting>();

        var client = factory.CreateClient();

        var token = await Login(client, user);

        // add the token to the client
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.Token}");


        // act
        // attempt to access the secured endpoint
        var response = await client.GetAsync("/api/secured/protected");

        // assert
        var responseContent = await response.Content.ReadAsStringAsync();

        WriteLine(responseContent);

        Assert.True(response.IsSuccessStatusCode);

        responseContent.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Secured_Protected_FailsWhenNotLoggedIn()
    {
        // arrange
        using var factory = new WebApplicationFactory<MarkerClassForTesting>();

        var client = factory.CreateClient();
                
        // act
        // attempt to access the secured endpoint
        var response = await client.GetAsync("/api/secured/protected");

        // assert
        var responseContent = await response.Content.ReadAsStringAsync();

        WriteLine(responseContent);

        Assert.False(response.IsSuccessStatusCode);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

}

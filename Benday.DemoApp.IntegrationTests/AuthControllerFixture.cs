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

    [Fact]
    public async Task Register_Post_DuplicateUser_Fails()
    {
        // arrange
        using var factory = new WebApplicationFactory<MarkerClassForTesting>();

        var client = factory.CreateClient();

        var now = DateTime.Now.Ticks.ToString();

        var username = $"email_{now}@email.com";

        var model = new RegisterModel()
        {
            Email = username,
            Password = PASSWORD
        };

        var postContent = new StringContent(
            JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");

        // create the user
        await client.PostAsync("/api/auth/register", postContent);

        // act
        // attempt to create the user again
        var response = await client.PostAsync("/api/auth/register", postContent);

        // assert
        var responseContent = await response.Content.ReadAsStringAsync();

        WriteLine(responseContent);

        Assert.False(response.IsSuccessStatusCode);

        var errors = JsonSerializer.Deserialize<List<AuthUsernameOperationFailure>>(
            responseContent);

        Assert.NotNull(errors);

        Assert.Equal(2, errors.Count);

        Assert.Contains(errors, x => x.Code == "DuplicateUserName");
        Assert.Contains(errors, x => x.Code == "DuplicateEmail");
    }

    [Fact]
    public async Task Login_Post_ValidUser()
    {
        // arrange
        using var factory = new WebApplicationFactory<MarkerClassForTesting>();

        var client = factory.CreateClient();

        var now = DateTime.Now.Ticks.ToString();

        var username = $"email_{now}@email.com";

        var model = new RegisterModel()
        {
            Email = username,
            Password = PASSWORD
        };

        var postContent = new StringContent(
            JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");

        // create the user
        await client.PostAsync("/api/auth/register", postContent);

        // act
        // attempt to login
        var response = await client.PostAsync("/api/auth/login", postContent);

        // assert
        var responseContent = await response.Content.ReadAsStringAsync();

        WriteLine(responseContent);

        Assert.True(response.IsSuccessStatusCode);

        Assert.False(string.IsNullOrWhiteSpace(responseContent));
    }

    [Fact]
    public async Task Login_Post_InvalidUserFails()
    {
        // arrange
        using var factory = new WebApplicationFactory<MarkerClassForTesting>();

        var client = factory.CreateClient();

        var now = DateTime.Now.Ticks.ToString();

        var username = $"bogus@email.com";

        var model = new LoginModel()
        {
            Email = username,
            Password = PASSWORD
        };

        var postContent = new StringContent(
            JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");

        // act
        // attempt to login
        var response = await client.PostAsync("/api/auth/login", postContent);

        // assert
        var responseContent = await response.Content.ReadAsStringAsync();

        WriteLine(responseContent);

        Assert.False(response.IsSuccessStatusCode);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        Assert.False(string.IsNullOrWhiteSpace(responseContent));
    }
}

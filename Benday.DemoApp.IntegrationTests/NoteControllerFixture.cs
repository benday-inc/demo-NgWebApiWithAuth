using Benday.DemoApp.Api;
using Benday.DemoApp.WebApi;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Text.Json;

namespace Benday.DemoApp.IntegrationTests;
public class NoteControllerFixture
{
    [Fact]
    public async Task Index_Get_WithOwnerId_ReturnsSuccess()
    {
        // arrange
        var factory = new WebApplicationFactory<MarkerClassForTesting>();

        var client = factory.CreateClient();

        var ownerId = "fakeownerid";

        // act
        var response = await client.GetAsync($"api/note/{ownerId}");

        // assert
        Assert.NotNull(response);

        var content = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode == false)
        {
            TestUtilities.CheckForDependencyError(content);
        }

        var notes = JsonSerializer.Deserialize<List<Note>>(content);

        Assert.True(response.IsSuccessStatusCode);
    }

    [Fact]
    public async Task Index_Post_NewNote()
    {
        // arrange
        var factory = new WebApplicationFactory<MarkerClassForTesting>();

        var client = factory.CreateClient();

        var now = DateTime.Now.Ticks.ToString();
        var ownerId = "fakeownerid";

        var note = new Note();
        note.Subject = $"subject_{now}";
        note.Text = $"text_{now}";
        note.OwnerId = ownerId;

        var json = JsonSerializer.Serialize(note);
        var contentToPost = 
            new StringContent(
                json, System.Text.Encoding.UTF8, "application/json");

        // act
        var response = await client.PostAsync($"api/note/{ownerId}", contentToPost);

        // assert
        Assert.NotNull(response);

        var content = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode == false)
        {
            TestUtilities.CheckForDependencyError(content);
        }

        response.EnsureSuccessStatusCode();
    }
}

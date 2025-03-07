using System.Text.Json.Serialization;

namespace Benday.DemoApp.IntegrationTests;
public class LoginTokenResponse
{
    [JsonPropertyName("token")]
    public string Token { get; set; } = string.Empty;

}
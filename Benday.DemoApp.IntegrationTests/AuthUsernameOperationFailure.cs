using System.Text.Json.Serialization;

namespace Benday.DemoApp.IntegrationTests;

public class AuthUsernameOperationFailure
{
    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

}


namespace Benday.DemoApp.WebApi;

public class CosmosConfig
{
    public string Key { get; set; } = string.Empty;
    public string Endpoint { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = string.Empty;

    public string ConnectionString
    {
        get
        {
            return $"AccountEndpoint={Endpoint};AccountKey={Key};";
        }
    }
}

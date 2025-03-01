using Benday.CosmosDb.DomainModels;

namespace Benday.DemoApp.Api;

public class Note : OwnedItemBase
{
    public string Subject { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
}

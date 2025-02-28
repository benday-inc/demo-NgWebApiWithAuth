
namespace Benday.DemoApp.WebApi;

public static class ExtensionMethods
{
    public static string ThrowIfEmptyOrNull(this string? value, string? valueName = null)
    {
        if (string.IsNullOrWhiteSpace(value) == true)
        {
            var message = "Value cannot be empty or null.";

            if (valueName != null)
            {
                message = $"Value for '{valueName}' cannot be empty or null.";
            }

            throw new InvalidOperationException(message);
        }

        return value;
    }
}


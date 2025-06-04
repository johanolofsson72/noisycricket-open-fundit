
using Microsoft.Extensions.Configuration;

namespace Shared.ApiKeys;

public class ApiKeyValidator(IConfiguration configuration) : IApiKeyValidator
{
    public bool Validate(string? apiKey)
    {
        // Retrieve the expected API key from the app settings
        var expectedApiKey = configuration.GetValue<string>("ApiKeys:ApiKey");

        // Compare the provided API key with the expected API key
        return apiKey == expectedApiKey;
    }
}
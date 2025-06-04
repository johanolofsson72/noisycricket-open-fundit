namespace Shared.ApiKeys;

public interface IApiKeyValidator
{
    bool Validate(string? apiKey);
}
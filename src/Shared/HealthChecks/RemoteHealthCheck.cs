
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Shared.HealthChecks;
public class RemoteHealthCheckOpenExchangeRate(IHttpClientFactory httpClientFactory, IConfiguration configuration) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
    {
        using var httpClient = httpClientFactory.CreateClient();
        var endpoint = configuration.GetValue<string>("OpenExchangeRatesUrl");
        var response = await httpClient.GetAsync(endpoint, cancellationToken);
        return response.IsSuccessStatusCode
            ? HealthCheckResult.Healthy($"Remote endpoint for OpenExchangeRate is healthy.")
            : HealthCheckResult.Unhealthy($"Remote endpoint for OpenExchangeRate is unhealthy");
    }
}
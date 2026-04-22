using Fiamma.Web.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace Fiamma.Web.HealthChecks;

public class ApiHealthCheck : IHealthCheck
{
    private readonly BaseUrlConfiguration _baseUrlConfiguration;

    public ApiHealthCheck(IOptions<BaseUrlConfiguration> baseUrlConfiguration)
    {
        _baseUrlConfiguration = baseUrlConfiguration.Value;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        string myUrl = _baseUrlConfiguration.ApiBase + "catalog-items";
        var client = new HttpClient();

        try
        {
            var response = await client.GetAsync(myUrl, cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                return HealthCheckResult.Healthy("The API responded successfully.");
            }

            return HealthCheckResult.Unhealthy($"The API responded with status code {(int)response.StatusCode}.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("The API health check request failed.", ex);
        }
    }
}


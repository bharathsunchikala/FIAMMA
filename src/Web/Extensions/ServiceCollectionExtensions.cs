using Azure.Identity;
using BlazorAdmin;
using BlazorAdmin.Services;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.DotNet.Scaffolding.Shared;
using Microsoft.EntityFrameworkCore;
using Fiamma.Infrastructure;
using Fiamma.Infrastructure.Data;
using Fiamma.Infrastructure.Identity;
using Fiamma.Web.Configuration;
using Fiamma.Web.HealthChecks;
using Yarp.ReverseProxy.Configuration;

namespace Fiamma.Web.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddDatabaseContexts(this IServiceCollection services, IWebHostEnvironment environment, ConfigurationManager configuration)
    {
        if (ShouldUseLocalDataStores(environment, configuration))
        {
            // Configure SQL Server (local)
            services.ConfigureLocalDatabaseContexts(configuration);
        }
        else
        {
            // Configure SQL Server (prod)
            var credential = new ChainedTokenCredential(new AzureDeveloperCliCredential(), new DefaultAzureCredential());
            configuration.AddAzureKeyVault(new Uri(configuration["AZURE_KEY_VAULT_ENDPOINT"] ?? ""), credential);

            services.AddDbContext<CatalogContext>((provider, options) =>
            {
                var connectionString = configuration[configuration["AZURE_SQL_CATALOG_CONNECTION_STRING_KEY"] ?? ""];
                options.UseSqlServer(connectionString, sqlOptions => sqlOptions.EnableRetryOnFailure())
                .AddInterceptors(provider.GetRequiredService<DbCallCountingInterceptor>());
            });
            services.AddDbContext<AppIdentityDbContext>((provider,options) =>
            {
                var connectionString = configuration[configuration["AZURE_SQL_IDENTITY_CONNECTION_STRING_KEY"] ?? ""];
                options.UseSqlServer(connectionString, sqlOptions => sqlOptions.EnableRetryOnFailure())
                                .AddInterceptors(provider.GetRequiredService<DbCallCountingInterceptor>());
            });
        }
    }

    public static void AddRenderForwardedHeaders(this IServiceCollection services, ConfigurationManager configuration)
    {
        if (!string.Equals(configuration["RENDER"], "true", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            options.KnownIPNetworks.Clear();
            options.KnownProxies.Clear();
        });
    }

    public static void AddCookieAuthentication(this IServiceCollection services)
    {
        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.SameSite = SameSiteMode.Lax;
            });
    }

    public static void AddCustomHealthChecks(this IServiceCollection services)
    {
        services
            .AddHealthChecks()
            .AddCheck<ApiHealthCheck>("api_health_check", tags: new[] { "apiHealthCheck" })
            .AddCheck<HomePageHealthCheck>("home_page_health_check", tags: new[] { "homePageHealthCheck" });
    }

    public static void AddBlazor(this IServiceCollection services, ConfigurationManager configuration)
    {
        var configSection = configuration.GetRequiredSection(BaseUrlConfiguration.CONFIG_NAME);
        services.Configure<BaseUrlConfiguration>(configSection);

        // Blazor Admin Required Services for Prerendering
        services.AddScoped<HttpClient>(s => new HttpClient
        {
            BaseAddress = new Uri("https+http://blazoradmin")
        });

        // add blazor services
        services.AddBlazoredLocalStorage();
        services.AddServerSideBlazor();
        services.AddScoped<ToastService>();
        services.AddScoped<HttpService>();
        services.AddBlazorServices();
    }

    public static void AddPublicApiReverseProxy(this IServiceCollection services, ConfigurationManager configuration)
    {
        var configSection = configuration.GetRequiredSection(BaseUrlConfiguration.CONFIG_NAME);
        var baseUrlConfiguration = configSection.Get<BaseUrlConfiguration>()
            ?? throw new InvalidOperationException("Missing baseUrls configuration.");

        if (string.IsNullOrWhiteSpace(baseUrlConfiguration.ApiBase))
        {
            throw new InvalidOperationException("The baseUrls:apiBase setting is required.");
        }

        var apiBaseUri = new Uri(baseUrlConfiguration.ApiBase, UriKind.Absolute);
        var destinationAddress = apiBaseUri.GetLeftPart(UriPartial.Authority);

        var routes = new[]
        {
            new RouteConfig
            {
                RouteId = "publicapi",
                ClusterId = "publicapi-cluster",
                Match = new RouteMatch
                {
                    Path = "/api/{**catch-all}"
                }
            }
        };

        var clusters = new[]
        {
            new ClusterConfig
            {
                ClusterId = "publicapi-cluster",
                Destinations = new Dictionary<string, DestinationConfig>(StringComparer.OrdinalIgnoreCase)
                {
                    ["publicapi"] = new() { Address = destinationAddress }
                }
            }
        };

        services.AddReverseProxy().LoadFromMemory(routes, clusters);
    }

    private static bool ShouldUseLocalDataStores(IWebHostEnvironment environment, ConfigurationManager configuration)
    {
        return environment.IsDevelopment()
            || environment.IsDocker()
            || string.Equals(configuration["RENDER"], "true", StringComparison.OrdinalIgnoreCase);
    }
}


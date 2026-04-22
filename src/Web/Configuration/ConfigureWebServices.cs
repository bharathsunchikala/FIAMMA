using Fiamma.ApplicationCore.Services;
using Fiamma.Web.Interfaces;
using Fiamma.Web.Services;

namespace Fiamma.Web.Configuration;

public static class ConfigureWebServices
{
    public static IServiceCollection AddWebServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Add MediatR support for the services
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(BasketViewModelService).Assembly);
            cfg.RegisterServicesFromAssembly(typeof(OrderService).Assembly);
        }
        );
        services.AddScoped<IBasketViewModelService, BasketViewModelService>();
        services.AddScoped<CatalogViewModelService>();
        services.AddScoped<ICatalogItemViewModelService, CatalogItemViewModelService>();
        services.Configure<CatalogSettings>(configuration);
        services.AddScoped<ICatalogViewModelService, CachedCatalogViewModelService>();

        return services;
    }
}


using Fiamma.ApplicationCore.Interfaces;
using Fiamma.ApplicationCore.Services;
using Fiamma.Infrastructure.Data;
using Fiamma.Infrastructure.Data.Queries;
using Fiamma.Infrastructure.Logging;
using Fiamma.Infrastructure.Services;

namespace Fiamma.Web.Configuration;

public static class ConfigureCoreServices
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped(typeof(IReadRepository<>), typeof(EfRepository<>));
        services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));

        services.AddScoped<IBasketService, BasketService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IBasketQueryService, BasketQueryService>();

        var catalogSettings = configuration.Get<CatalogSettings>() ?? new CatalogSettings();
        services.AddSingleton<IUriComposer>(new UriComposer(catalogSettings));

        services.AddScoped(typeof(IAppLogger<>), typeof(LoggerAdapter<>));
        services.AddTransient<IEmailSender, LoggerEmailSender>();

        return services;
    }
}


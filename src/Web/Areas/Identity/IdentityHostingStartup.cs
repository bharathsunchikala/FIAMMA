[assembly: HostingStartup(typeof(Fiamma.Web.Areas.Identity.IdentityHostingStartup))]
namespace Fiamma.Web.Areas.Identity;

public class IdentityHostingStartup : IHostingStartup
{
    public void Configure(IWebHostBuilder builder)
    {
        builder.ConfigureServices((context, services) =>
        {
        });
    }
}


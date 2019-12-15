using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(Recitopia.Areas.Identity.IdentityHostingStartup))]
namespace Recitopia.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
            });
        }
    }
}
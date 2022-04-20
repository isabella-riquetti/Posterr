using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Posterr.DB
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
            => services.AddDbContext<ApiContext>();

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            this.Configure(app, env);
        }
    }
}

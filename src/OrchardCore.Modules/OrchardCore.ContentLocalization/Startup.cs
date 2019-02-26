using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.ContentLocalization.Model;
using OrchardCore.Modules;

namespace OrchardCore.Alias
{
    public class Startup : StartupBase
    {
        static Startup()
        {
        }

        public override void Configure(IApplicationBuilder app, IRouteBuilder routes, IServiceProvider serviceProvider)
        {
            base.Configure(app, routes, serviceProvider);
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<ContentPart, LocalizationPart>();

        }
    }
}

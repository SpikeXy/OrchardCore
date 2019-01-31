using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Environment.Shell;

namespace OrchardCore.Media
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMedia(this IServiceCollection services)
        {
            services.AddSingleton<IMediaService, MediaService>();

            services.AddScoped<IMediaFactorySelector, ImageFactorySelector>();

            return services;
        }

        public static IServiceCollection AddMediaEventHandlers(this IServiceCollection services)
        {
            services.AddSingleton<IShellEventHandler, DeleteMediaShellEventHandler>();

            return services;
        }
    }
}
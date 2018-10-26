using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Apis.GraphQL;
using OrchardCore.ContentManagement.GraphQL.Queries;
using OrchardCore.ContentManagement.GraphQL.Queries.Types;
using OrchardCore.Security.Permissions;
using OrchardCore.Apis;
using OrchardCore.ContentManagement.GraphQL.Queries.Filters;

namespace OrchardCore.ContentManagement.GraphQL
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddContentGraphQL(this IServiceCollection services)
        {
            services.AddSingleton<ISchemaBuilder, ContentItemQuery>();
            services.AddSingleton<ISchemaBuilder, ContentTypeQuery>();

            services.AddTransient<ContentItemType>();

            services.AddScoped<IPermissionProvider, Permissions>();

            services.AddTransient<DynamicPartGraphType>();
            services.AddScoped<IContentTypeBuilder, TypedContentTypeBuilder>();
            services.AddScoped<IContentTypeBuilder, DynamicContentTypeBuilder>();

            services.AddGraphQLFilterType<ContentItem, OrderByFilter>();

            return services;
        }
    }
}

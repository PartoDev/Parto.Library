using Microsoft.Extensions.DependencyInjection;
using Parto.Connection.Abstractions;

namespace Parto.Connection.Manager
{
    public static class ConnectionManagerExtension
    {
        public static IServiceCollection AddConnectionManager(this IServiceCollection serviceCollection) =>
            serviceCollection.AddSingleton<IConnectionManager, ConnectionManager>();

        public static IConnectionMiddlewareBuilder UseConnectionManager(
            this IConnectionMiddlewareBuilder builder) => builder.UseMiddleware<IConnectionManager>();
    }
}
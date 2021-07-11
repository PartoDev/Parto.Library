using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Parto.Connection.Abstractions;

namespace Parto.Connection.Server
{
    public static class ConnectionServerExtension
    {
        public static IServiceCollection AddConnectionServer(this IServiceCollection serviceCollection,
            ConnectionMiddlewareDelegate connectionMiddlewareDelegate) =>
            serviceCollection.AddSingleton<IConnectionServer, ConnectionServer>(provider =>
                ActivatorUtilities.CreateInstance<ConnectionServer>(provider, ConnectionSocketExtension.CreateConnectionInvoker(connectionMiddlewareDelegate)));

        public static IApplicationBuilder
            UseConnectionWebSocketListener(this IApplicationBuilder builder, string path) =>
            builder.UseMiddleware<ConnectionWebSocketListenerMiddleware>(path);
    }
}
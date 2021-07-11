using Microsoft.Extensions.DependencyInjection;
using Parto.Connection.Abstractions;

namespace Parto.Connection.Client
{
    public static class ConnectionClientExtension
    {
        public static IServiceCollection AddConnectionClient(this IServiceCollection serviceCollection,
            ConnectionMiddlewareDelegate connectionMiddlewareDelegate) =>
            serviceCollection.AddSingleton<IConnectionClient, ConnectionClient>(provider =>
                ActivatorUtilities.CreateInstance<ConnectionClient>(provider,
                    ConnectionSocketExtension.CreateConnectionInvoker(connectionMiddlewareDelegate)));
    }
}
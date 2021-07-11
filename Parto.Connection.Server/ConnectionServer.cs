using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Parto.Connection.Abstractions;

namespace Parto.Connection.Server
{
    public class ConnectionServer : IConnectionServer
    {
        private readonly Func<NextConnectionDelegate, NextConnectionDelegate> func;
        private readonly IServiceProvider serviceProvider;

        public ConnectionServer(IServiceProvider serviceProvider,
            Func<NextConnectionDelegate, NextConnectionDelegate> func)
        {
            this.serviceProvider = serviceProvider;
            this.func = func;
        }

        public async ValueTask AcceptWebSocketAsync(WebSocket webSocket,
            CancellationToken cancellationToken = default)
        {
            var serviceScope = serviceProvider.CreateScope();
            using ConnectionContext connectionContext = new WebSocketConnectionContext(webSocket,
                serviceScope.ServiceProvider, cancellationToken);
            await ConnectionSocketExtension.InvokeConnectionDelegateAsync(func, connectionContext,
                (socketContext, _) =>
                {
                    socketContext.Dispose();
                    return ValueTask.CompletedTask;
                },
                cancellationToken).ConfigureAwait(false);
        }
    }
}
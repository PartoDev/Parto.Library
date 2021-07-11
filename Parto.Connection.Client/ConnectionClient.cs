using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Parto.Connection.Abstractions;

namespace Parto.Connection.Client
{
    public class ConnectionClient : IConnectionClient
    {
        private readonly Func<NextConnectionDelegate, NextConnectionDelegate> func;
        private readonly IServiceProvider serviceProvider;
        public readonly ClientWebSocket webSocket = new();

        public ConnectionClient(IServiceProvider serviceProvider,
            Func<NextConnectionDelegate, NextConnectionDelegate> func)
        {
            this.serviceProvider = serviceProvider;
            this.func = func;
        }

        public async ValueTask ConnectToWebSocketAsync(Uri uri, CancellationToken cancellationToken)
        {
            await webSocket.ConnectAsync(uri, cancellationToken).ConfigureAwait(false);
            using ConnectionContext connectionContext = new WebSocketConnectionContext(webSocket,
                serviceProvider
                    .CreateScope()
                    .ServiceProvider,
                cancellationToken);

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
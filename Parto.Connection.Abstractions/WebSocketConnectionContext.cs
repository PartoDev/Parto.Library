using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Parto.Connection.Abstractions
{
    public class WebSocketConnectionContext : ConnectionContext
    {
        public WebSocketConnectionContext(WebSocket webSocket, IServiceProvider serviceProvider,
            params CancellationToken[] cancellationTokens) : base(serviceProvider, cancellationTokens) =>
            WebSocket = webSocket;

        public WebSocket WebSocket { get; }

        public override void Dispose()
        {
            WebSocket.Dispose();
            base.Dispose();
            GC.SuppressFinalize(this);
        }

        public override async IAsyncEnumerable<IConnectionBlockModel?> BlockReceiverAsync(
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            while (WebSocket.State == WebSocketState.Open)
                yield return await WebSocket.ReceiveBlockAsync(cancellationToken);
        }

        public override async IAsyncEnumerable<ConnectionBlockModel<TModel>?> BlockReceiverAsync<TModel>(
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            while (WebSocket.State == WebSocketState.Open)
                yield return await WebSocket.ReceiveBlockAsync<TModel>(cancellationToken);
        }

        public override async ValueTask<IConnectionBlockModel?> ReceiveBlockAsync(
            CancellationToken cancellationToken = default) =>
            await WebSocket.ReceiveBlockAsync(cancellationToken);

        public override async ValueTask<ConnectionBlockModel<TModel>?> ReceiveBlockAsync<TModel>(
            CancellationToken cancellationToken = default) =>
            await WebSocket.ReceiveBlockAsync<TModel>(cancellationToken);

        public override ValueTask SendBlockAsync(IConnectionBlockModel block,
            CancellationToken cancellationToken = default) =>
            WebSocket.SendBlockAsync(block, cancellationToken);
    }
}
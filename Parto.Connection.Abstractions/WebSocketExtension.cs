using System;
using System.IO;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace Parto.Connection.Abstractions
{
    public static class WebSocketExtension
    {
        private const int BufferSize = 4096;


        public static async Task<MemoryStream> ReceiveMemoryStream(this WebSocket webSocket,
            CancellationToken cancellation = default)
        {
            MemoryStream memoryStream = new();
            ValueWebSocketReceiveResult valueWebSocketReceiveResult = default;
            while (!valueWebSocketReceiveResult.EndOfMessage)
            {
                byte[] bytes = new byte[BufferSize];
                Memory<byte> buffer = new(bytes);
                valueWebSocketReceiveResult =
                    await webSocket.ReceiveAsync(buffer, cancellation)
                        .ConfigureAwait(false);
                var length = valueWebSocketReceiveResult.Count;
                if (buffer.Length != length) buffer = buffer[..length];

                await memoryStream.WriteAsync(buffer, cancellation)
                    .ConfigureAwait(false);
            }

            return memoryStream;
        }

        public static async ValueTask<byte[]> ReceiveAsync(this WebSocket webSocket,
            CancellationToken cancellation = default)
        {
            await using var memoryStream = await webSocket.ReceiveMemoryStream(cancellation)
                .ConfigureAwait(false);
            return memoryStream.ToArray();
        }

        public static async ValueTask SendAsync(this WebSocket webSocket,
            byte[] bytes,
            CancellationToken cancellation = default)
        {
            await using MemoryStream memoryStream = new(bytes);
            await webSocket.SendAsync(memoryStream, cancellation)
                .ConfigureAwait(false);
        }

        public static async ValueTask SendAsync(this WebSocket webSocket,
            Stream stream,
            CancellationToken cancellation = default)
        {
            stream.Seek(0, SeekOrigin.Begin);
            var completed = false;
            var endPosition = stream.Length;
            while (!completed)
            {
                byte[] bytes = new byte[BufferSize];
                Memory<byte> buffer = new(bytes);
                var length = await stream.ReadAsync(buffer, cancellation)
                    .ConfigureAwait(false);
                if (length != buffer.Length) buffer = buffer[..length];

                completed = stream.Position == endPosition;
                await webSocket.SendAsync(buffer, WebSocketMessageType.Text, completed, cancellation);
            }
        }

        public static async ValueTask SendBlockAsync(this WebSocket webSocket,
            IConnectionBlockModel model,
            CancellationToken cancellationToken = default)
        {
            await using MemoryStream memoryStream = new();
            await model.SerializeAsync(memoryStream, cancellationToken)
                .ConfigureAwait(false);
            await SendAsync(webSocket, memoryStream, cancellationToken)
                .ConfigureAwait(false);
        }

        public static async ValueTask TransferBlockAsync(this WebSocket webSocket,
            IConnectionBlockModel model,
            CancellationToken cancellationToken = default)
        {
            await using MemoryStream memoryStream = new();
            await model.SerializeAsync(memoryStream, cancellationToken)
                .ConfigureAwait(false);
            await SendAsync(webSocket, memoryStream, cancellationToken)
                .ConfigureAwait(false);
        }


        public static async ValueTask<ConnectionBlockModel?> ReceiveBlockAsync(this WebSocket webSocket,
            CancellationToken cancellationToken = default)
        {
            await using var stream = await webSocket.ReceiveMemoryStream(cancellationToken)
                .ConfigureAwait(false);
            stream.Seek(0, SeekOrigin.Begin);
            return await ConnectionBlockModel.DeserializeAsync(stream, cancellationToken)
                .ConfigureAwait(false);
        }

        public static async ValueTask<ConnectionBlockModel<TModel>?> ReceiveBlockAsync<TModel>(this WebSocket webSocket,
            CancellationToken cancellationToken = default)
        {
            await using var stream = await webSocket.ReceiveMemoryStream(cancellationToken)
                .ConfigureAwait(false);
            stream.Seek(0, SeekOrigin.Begin);
            return await ConnectionBlockModel<TModel>.DeserializeAsync(stream, cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Parto.Connection.Server
{
    public class ConnectionWebSocketListenerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _path;

        public ConnectionWebSocketListenerMiddleware(RequestDelegate next, string path)
        {
            _next = next;
            _path = path;
        }

        public async Task InvokeAsync(HttpContext context,
            IConnectionServer connectionServer
        )
        {
            if (context.WebSockets.IsWebSocketRequest &&
                context.Request.Path == _path)
            {
                var cancellationToken = context.RequestAborted;
                var webSocket = await context.WebSockets.AcceptWebSocketAsync()
                    .ConfigureAwait(false);
                await connectionServer
                    .AcceptWebSocketAsync(webSocket, cancellationToken)
                    .ConfigureAwait(false);
            }

            await _next(context)
                .ConfigureAwait(false);
        }
    }
}
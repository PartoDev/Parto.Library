using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace Parto.Connection.Server
{
    public interface IConnectionServer
    {
        ValueTask AcceptWebSocketAsync(WebSocket webSocket, CancellationToken cancellationToken = default);
    }
}
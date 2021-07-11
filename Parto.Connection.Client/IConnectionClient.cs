using System;
using System.Threading;
using System.Threading.Tasks;

namespace Parto.Connection.Client
{
    public interface IConnectionClient
    {
        ValueTask ConnectToWebSocketAsync(Uri uri, CancellationToken cancellationToken = default);
    }
}
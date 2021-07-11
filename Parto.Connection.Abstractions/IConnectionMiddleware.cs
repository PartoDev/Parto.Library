using System.Threading;
using System.Threading.Tasks;

namespace Parto.Connection.Abstractions
{
    public interface IConnectionMiddleware
    {
        public ValueTask InvokeConnectionAsync(IConnectionContext context,
            NextConnectionDelegate next,
            CancellationToken cancellationToken);
    }
}
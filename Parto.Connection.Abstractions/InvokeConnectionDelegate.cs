using System.Threading;
using System.Threading.Tasks;

namespace Parto.Connection.Abstractions
{
    public delegate ValueTask InvokeConnectionDelegate(IConnectionContext context,
        NextConnectionDelegate next,
        CancellationToken cancellationToken);
}
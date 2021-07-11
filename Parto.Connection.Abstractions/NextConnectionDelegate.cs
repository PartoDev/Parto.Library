using System.Threading;
using System.Threading.Tasks;

namespace Parto.Connection.Abstractions
{
    public delegate ValueTask
        NextConnectionDelegate(IConnectionContext context, CancellationToken cancellationToken);
}
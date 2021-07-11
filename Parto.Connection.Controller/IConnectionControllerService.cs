using System.Threading;
using System.Threading.Tasks;
using Parto.Connection.Abstractions;

namespace Parto.Connection.Controller
{
    public interface IConnectionControllerService : IConnectionMiddleware
    {
        ValueTask InvokeControllerAsync(IConnectionContext context, string path,
            CancellationToken cancellationToken = default);

        ValueTask InvokeControllerAsync(IConnectionContext context, IConnectionBlockModel? block,
            CancellationToken cancellationToken = default);
    }
}
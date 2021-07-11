using System.Threading;
using System.Threading.Tasks;

namespace Parto.Connection.Controller.Abstractions
{
    public delegate ValueTask ConnectionStepNext(IConnectionController controller,
        CancellationToken cancellationToken);
}
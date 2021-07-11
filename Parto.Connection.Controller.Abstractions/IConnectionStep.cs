using System.Threading;
using System.Threading.Tasks;

namespace Parto.Connection.Controller.Abstractions
{
    public interface IConnectionStep
    {
        ConnectionStepType StepType { get; }

        ValueTask StepInvokeAsync(IConnectionController controller,
            ConnectionStepNext next,
            CancellationToken cancellationToken = default);
    }
}
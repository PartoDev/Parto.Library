using System;
using System.Threading;
using System.Threading.Tasks;

namespace Parto.Connection.Controller.Abstractions
{
    [AttributeUsage(AttributeTargets.Method)]
    public abstract class ConnectionStep : Attribute, IConnectionStep
    {
        protected ConnectionStep(ConnectionStepType stepType) => StepType = stepType;

        public ConnectionStepType StepType { get; }

        public abstract ValueTask StepInvokeAsync(IConnectionController controller,
            ConnectionStepNext next,
            CancellationToken cancellationToken = default);
    }
}
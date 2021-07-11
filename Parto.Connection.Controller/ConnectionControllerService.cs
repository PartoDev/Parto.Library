using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Parto.Connection.Abstractions;
using Parto.Connection.Controller.Abstractions;

namespace Parto.Connection.Controller
{
    public class ConnectionControllerService : IConnectionControllerService
    {
        private readonly ConnectionLocation _connectionLocation;
        private readonly ConnectionControllerOptions connectionControllerOptions;

        public ConnectionControllerService(ConnectionLocation connectionLocation,
            ConnectionControllerOptions connectionControllerOptions)
        {
            _connectionLocation = connectionLocation;
            this.connectionControllerOptions = connectionControllerOptions;
        }

        public async ValueTask InvokeConnectionAsync(IConnectionContext context,
            NextConnectionDelegate next,
            CancellationToken cancellationToken)
        {
            await foreach (var block in context.BlockReceiverAsync(cancellationToken))
            {
                await OnBlockAsync(context, block, cancellationToken);
            }

            await next(context, cancellationToken)
                .ConfigureAwait(false);
        }

        private async ValueTask OnBlockAsync(IConnectionContext context,
            IConnectionBlockModel? block, CancellationToken cancellationToken = default)
        {
            if (block == null)
            {
                await ReceiveBlockNullErrorAsync(context, cancellationToken);
                return;
            }

            var socketLocation = GetSocketLocation(block, _connectionLocation);
            if (socketLocation == null)
            {
                await ReceiveLocationNUllErrorAsync(context, block, cancellationToken);
                return;
            }

            var socketController = GetSocketController(context, block, socketLocation);
            if (socketController == null)
            {
                await ReceiveControllerNullErrorAsync(context, block, cancellationToken);
                return;
            }

            await InvokeControllerAsync(socketController, cancellationToken);
        }

        private async ValueTask ReceiveControllerNullErrorAsync(IConnectionContext context,
            IConnectionBlockModel block, CancellationToken cancellationToken = default)
        {
            if (!string.IsNullOrEmpty(connectionControllerOptions.ReceiveControllerNullErrorPath))
            {
                block.Path = connectionControllerOptions.ReceiveControllerNullErrorPath;
                await InvokeControllerAsync(context, block, cancellationToken);
            }
        }

        private async ValueTask ReceiveLocationNUllErrorAsync(IConnectionContext context,
            IConnectionBlockModel block, CancellationToken cancellationToken = default)
        {
            if (!string.IsNullOrEmpty(connectionControllerOptions.ReceiveLocationNUllErrorPath))
            {
                block.Path = connectionControllerOptions.ReceiveLocationNUllErrorPath;
                await InvokeControllerAsync(context, block, cancellationToken);
            }
        }

        private async ValueTask ReceiveBlockNullErrorAsync(IConnectionContext context,
            CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(connectionControllerOptions.ReceiveBlockNullErrorPath))
                await InvokeControllerAsync(context, connectionControllerOptions.ReceiveBlockNullErrorPath,
                    cancellationToken);
        }

        public ValueTask InvokeControllerAsync(IConnectionContext context, string path,
            CancellationToken cancellationToken = default) =>
            InvokeControllerAsync(context, new ConnectionBlockModel(path, null), cancellationToken);

        public async ValueTask InvokeControllerAsync(IConnectionContext context, IConnectionBlockModel? block,
            CancellationToken cancellationToken = default)
        {
            if (block == null) return;

            var socketLocation = GetSocketLocation(block, _connectionLocation);
            if (socketLocation == null) return;

            var socketController = GetSocketController(context, block, socketLocation);
            if (socketController == null) return;

            await InvokeControllerAsync(socketController, cancellationToken);
        }

        private static async ValueTask InvokeControllerAsync(
            IConnectionController controller,
            CancellationToken cancellationToken = default) => await InvokeStepAsync(controller,
                ConnectionStepType.Before,
                InvokeControllerBeforeStepAsync,
                cancellationToken)
            .ConfigureAwait(false);

        private static async ValueTask InvokeControllerBeforeStepAsync(IConnectionController controller,
            CancellationToken cancellationToken = default) => await InvokeStepAsync(controller,
                ConnectionStepType.BeforeParameter,
                InvokeControllerBeforeParameterStepAsync,
                cancellationToken)
            .ConfigureAwait(false);

        private static async ValueTask InvokeControllerBeforeParameterStepAsync(IConnectionController controller,
            CancellationToken cancellationToken = default) => await InvokeStepAsync(controller,
                ConnectionStepType.AfterParameter,
                InvokeControllerAfterParameterStepAsync,
                cancellationToken)
            .ConfigureAwait(false);

        private static async ValueTask InvokeControllerAfterParameterStepAsync(IConnectionController controller,
            CancellationToken cancellationToken = default)
        {
            CreateMethodParameters(controller, cancellationToken);
            await InvokeStepAsync(controller,
                    ConnectionStepType.BeforeExecute,
                    InvokeControllerBeforeExecuteStepAsync,
                    cancellationToken)
                .ConfigureAwait(false);
        }

        private static async ValueTask InvokeControllerBeforeExecuteStepAsync(IConnectionController controller,
            CancellationToken cancellationToken = default) => await InvokeStepAsync(controller,
                ConnectionStepType.AfterExecute,
                InvokeControllerAfterExecuteStepAsync,
                cancellationToken)
            .ConfigureAwait(false);

        private static async ValueTask InvokeControllerAfterExecuteStepAsync(IConnectionController controller,
            CancellationToken cancellationToken = default)
        {
            controller.Context.Response.Block.Body =
                await InvokeMethod(controller);
            if (controller.Context.Response.Block.Parameters.Count != 0 ||
                controller.Context.Response.Block.Body != null)
                await InvokeStepAsync(controller,
                        ConnectionStepType.BeforeResult,
                        InvokeControllerBeforeResultStepAsync,
                        cancellationToken)
                    .ConfigureAwait(false);
            else
                await InvokeStepAsync(controller,
                        ConnectionStepType.After,
                        InvokeControllerAfterStepAsync,
                        cancellationToken)
                    .ConfigureAwait(false);
        }

        private static async ValueTask InvokeControllerBeforeResultStepAsync(IConnectionController controller,
            CancellationToken cancellationToken = default) => await InvokeStepAsync(controller,
                ConnectionStepType.AfterResult,
                InvokeControllerAfterResultStepAsync,
                cancellationToken)
            .ConfigureAwait(false);

        private static async ValueTask InvokeControllerAfterResultStepAsync(IConnectionController controller,
            CancellationToken cancellationToken = default)
        {
            await controller.Context.ConnectionContext.SendBlockAsync(controller.Context.Response.Block,
                cancellationToken);
            await InvokeStepAsync(controller,
                    ConnectionStepType.After,
                    InvokeControllerAfterStepAsync,
                    cancellationToken)
                .ConfigureAwait(false);
        }

        private static ValueTask InvokeControllerAfterStepAsync(IConnectionController controller,
            CancellationToken cancellationToken = default) => ValueTask.CompletedTask;

        //todo: fix this method!! very important!!
        private static async ValueTask<object?> InvokeMethod(IConnectionController controller)
        {
            dynamic? invoked = controller.Context.ConnectionLocation.Method?.Invoke(controller,
                controller.Context.Request.MethodParameters.Count != 0
                    ? controller.Context.Request.MethodParameters.ToArray()
                    : null);
            if (invoked is null)
                return null;
            Type type = invoked.GetType();
            if (type.FullName!.StartsWith(typeof(ValueTask<>).FullName!) ||
                type.FullName.StartsWith(typeof(Task<>).FullName!))
                return await invoked;
            if (!type.FullName.StartsWith(typeof(ValueTask).FullName!) &&
                !type.FullName.StartsWith(typeof(Task).FullName!)) return invoked;
            await invoked;
            return null;
        }

        private static void CreateMethodParameters(IConnectionController controller,
            CancellationToken cancellationToken = default)
        {
            foreach (var parameter in controller.Context.ConnectionLocation.Parameters)
                SetParameter(controller, parameter, cancellationToken);
        }

        private static void SetParameter(
            IConnectionController controller,
            ConnectionMethodParameter parameter,
            CancellationToken cancellationToken = default)
        {
            switch (parameter.ParameterType)
            {
                case ConnectionMethodParameterType.Parameter:
                    SetParameterCaseParameter(controller.Context.Request.Block.Parameters,
                        parameter,
                        controller.Context.Request.MethodParameters);
                    break;
                case ConnectionMethodParameterType.Body:
                    SetParameterCaseBody(controller.Context.Request.Block,
                        parameter,
                        controller.Context.Request.MethodParameters);
                    break;
                case ConnectionMethodParameterType.Cancellation:
                    controller.Context.Request.MethodParameters.Add(cancellationToken);
                    break;
                default:
                    throw new("not valid parameter type");
            }
        }

        private static void SetParameterCaseBody(IConnectionBlockModel block,
            ConnectionMethodParameter parameter,
            ICollection<object?> createParameters) => createParameters.Add(
            block.GetBody(parameter.Type) ??
            parameter.Default);

        private static void SetParameterCaseParameter(IDictionary<string, object>? parameters,
            ConnectionMethodParameter parameter,
            ICollection<object?> createParameters)
        {
            object? getParameter = null;
            if (parameters != null)
            {
                var newParameter = parameters[parameter.Name!];
                if (newParameter.GetType() == parameter.Type)
                    getParameter = newParameter;
                else
                    throw new("Not valid parameter");
            }

            createParameters.Add(getParameter ?? parameter.Default);
        }

        private static ConnectionController? GetSocketController(IConnectionContext context,
            IConnectionBlockModel block,
            ConnectionLocation connectionLocation)
        {
            var controllerType = connectionLocation!.ControllerType;
            if (controllerType == null) return null;

            if (context.ServiceProvider.GetService(controllerType) is not ConnectionController controller) return null;

            controller.Context = new ConnectionControllerContext(context,
                new ConnectionControllerRequestContext(block),
                new ConnectionControllerResponseContext(new(block.Path, block.Guid)),
                connectionLocation);
            return controller;
        }

        private static ConnectionLocation? GetSocketLocation(IConnectionBlockBaseModel block,
            ConnectionLocation mainConnectionLocation)
        {
            if (string.IsNullOrEmpty(block.Path)) return null;

            var socketLocation = mainConnectionLocation;
            return block.Path.Split("/")
                .Any(location => !socketLocation.Locations.TryGetValue(location, out socketLocation))
                ? null
                : socketLocation;
        }

        private static async ValueTask InvokeStepMethodAsync(IConnectionController controller,
            LinkedListNode<IConnectionStep>? step,
            ConnectionStepNext connectionStepNext,
            CancellationToken cancellationToken = default)
        {
            if (step != null)
                await step.Value.StepInvokeAsync(controller,
                        async (context, innerCancellationToken) =>
                            await InvokeStepMethodAsync(context, step.Next, connectionStepNext, innerCancellationToken)
                                .ConfigureAwait(false),
                        cancellationToken)
                    .ConfigureAwait(false);
            else
                await connectionStepNext(controller, cancellationToken)
                    .ConfigureAwait(false);
        }

        private static async ValueTask InvokeStepAsync(
            IConnectionController controller,
            ConnectionStepType connectionStepType,
            ConnectionStepNext connectionStepNext,
            CancellationToken cancellationToken = default)
        {
            if (controller.Context.ConnectionLocation.Steps.TryGetValue(connectionStepType, out var steps) &&
                steps != null)
                await InvokeStepMethodAsync(controller, steps.First, connectionStepNext, cancellationToken)
                    .ConfigureAwait(false);
            else
                await connectionStepNext(controller, cancellationToken)
                    .ConfigureAwait(false);
        }
    }
}
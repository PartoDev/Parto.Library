using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Parto.Connection.Abstractions;
using Parto.Connection.Controller.Abstractions;

namespace Parto.Connection.Controller
{
    public static class ConnectionControllerServiceExtension
    {
        public static IConnectionMiddlewareBuilder UseEndPointConnectionController(
            this IConnectionMiddlewareBuilder builder) => builder.UseMiddleware<IConnectionControllerService>();

        public static IServiceCollection AddConnectionController(this IServiceCollection serviceCollection,
            Action<ConnectionControllerOptions>? action = default)
        {
            ConnectionControllerOptions connectionControllerOptions = new();
            action?.Invoke(connectionControllerOptions);
            ConnectionLocation connectionLocation = new();
            foreach (var controller in Assembly.GetCallingAssembly()
                .GetTypes()
                .Where(x => x.IsSubclassOf(typeof(ConnectionController))))
                AddController(serviceCollection, controller, connectionLocation);

            return serviceCollection.AddSingleton<IConnectionControllerService, ConnectionControllerService>(provider =>
                ActivatorUtilities.CreateInstance<ConnectionControllerService>(provider, connectionLocation,
                    connectionControllerOptions));
        }

        private static IEnumerable<string> GetPath(MemberInfo type) =>
            type.GetCustomAttribute<ConnectionPathAttribute>()
                ?.Locations ??
            Array.Empty<string>();

        private static void AddController(IServiceCollection serviceCollection,
            Type controller,
            ConnectionLocation connectionLocation)
        {
            serviceCollection.AddScoped(controller);
            foreach (var method in controller.GetMethods()
                .Where(x => x.IsPublic))
                AddMethod(controller, connectionLocation, GetPath(controller), method);
        }

        private static void AddMethod(Type controller,
            ConnectionLocation connectionLocation,
            IEnumerable<string> controllerPath,
            MethodInfo method)
        {
            List<string> path = new();
            var methodPath = GetPath(method);
            var methodPathArray = methodPath as string[] ?? methodPath.ToArray();
            //todo: method with out path is not valid
            if (!methodPathArray.Any()) return;

            path.AddRange(controllerPath);
            path.AddRange(methodPathArray);
            var location = connectionLocation;
            foreach (var locationString in path)
                if (location!.Locations.TryGetValue(locationString, out var newLocation))
                    location = newLocation;
                else
                    location!.Locations.Add(locationString, location = new());

            location!.Method = method;
            location.ControllerType = controller;
            foreach (var socketStep in method.GetCustomAttributes<ConnectionStep>())
                //todo: check this code
                (connectionLocation.Steps[socketStep!.StepType] ??= new()).AddLast(socketStep);

            foreach (var parameter in method.GetParameters()) AddParameter(controller, method, parameter, location);
        }

        private static void AddParameter(Type controller,
            MethodInfo method,
            ParameterInfo parameter,
            ConnectionLocation location)
        {
            var parameterAttribute = parameter.GetCustomAttribute<ConnectionParameterAttribute>();
            if (parameterAttribute is not null)
                location.Parameters.Add(new
                (
                    (parameterAttribute.Name ?? parameter.Name)!,
                    ConnectionMethodParameterType.Parameter,
                    parameter.ParameterType,
                    parameter.DefaultValue
                ));
            else if (parameter.ParameterType == typeof(CancellationToken))
                location.Parameters.Add(new
                (
                    parameter.Name!,
                    ConnectionMethodParameterType.Cancellation,
                    parameter.ParameterType,
                    parameter.DefaultValue
                ));
            else if (parameter.GetCustomAttribute<ConnectionParameterAttribute>() is not null ||
                     location.Parameters.All(x => x.ParameterType != ConnectionMethodParameterType.Body))
                location.Parameters.Add(new
                (
                    parameter.Name!,
                    ConnectionMethodParameterType.Body,
                    parameter.ParameterType,
                    parameter.DefaultValue));
            else
                throw new("Parameter type not valid", new(method.ToString(), new(controller.ToString())));
        }
    }
}
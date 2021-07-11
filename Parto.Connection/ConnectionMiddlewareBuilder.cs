using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Parto.Connection.Abstractions;

namespace Parto.Connection
{
    internal class ConnectionMiddlewareBuilder : IConnectionMiddlewareBuilder
    {
        private readonly List<Func<NextConnectionDelegate, NextConnectionDelegate>> _invokeConnectionDelegates;

        public ConnectionMiddlewareBuilder(
            List<Func<NextConnectionDelegate, NextConnectionDelegate>> invokeConnectionDelegates) =>
            _invokeConnectionDelegates = invokeConnectionDelegates;

        public IConnectionMiddlewareBuilder Use(Func<NextConnectionDelegate, NextConnectionDelegate> func)
        {
            _invokeConnectionDelegates.Add(func);
            return this;
        }

        public IConnectionMiddlewareBuilder UseMiddleware<TSocketMiddleware>()
            where TSocketMiddleware : IConnectionMiddleware
        {
            Use(next => (context, token) => ActivatorUtilities
                .GetServiceOrCreateInstance<TSocketMiddleware>(context.ServiceProvider)
                .InvokeConnectionAsync(context, next, token));
            return this;
        }
    }
}
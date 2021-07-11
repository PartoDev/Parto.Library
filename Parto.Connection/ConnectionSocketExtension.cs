using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Parto.Connection.Abstractions;

namespace Parto.Connection
{
    public static class ConnectionSocketExtension
    {
        public static Func<NextConnectionDelegate, NextConnectionDelegate> CreateConnectionInvoker(
            ConnectionMiddlewareDelegate connectionMiddlewareDelegate)
        {
            List<Func<NextConnectionDelegate, NextConnectionDelegate>> invokeConnectionDelegates = new();
            ConnectionMiddlewareBuilder socketConnectionMiddlewareBuilder =
                new(invokeConnectionDelegates);
            connectionMiddlewareDelegate(socketConnectionMiddlewareBuilder);
            Func<NextConnectionDelegate, NextConnectionDelegate> invokeConnectionDelegate =
                next => next;
            foreach (var connectionDelegate in invokeConnectionDelegates)
            {
                var @delegate = (Func<NextConnectionDelegate, NextConnectionDelegate>)invokeConnectionDelegate.Clone();
                invokeConnectionDelegate = next => @delegate(connectionDelegate(next));
            }

            return invokeConnectionDelegate;
        }

        public static ValueTask InvokeConnectionDelegateAsync(
            Func<NextConnectionDelegate, NextConnectionDelegate> invokeConnectionDelegate, IConnectionContext context,
            NextConnectionDelegate next,
            CancellationToken cancellationToken) => invokeConnectionDelegate.Invoke(next)
            .Invoke(context, cancellationToken);
    }
}
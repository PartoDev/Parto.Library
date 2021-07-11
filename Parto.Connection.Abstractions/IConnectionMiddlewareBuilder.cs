using System;

namespace Parto.Connection.Abstractions
{
    public interface IConnectionMiddlewareBuilder
    {
        IConnectionMiddlewareBuilder Use(Func<NextConnectionDelegate, NextConnectionDelegate> func);

        IConnectionMiddlewareBuilder UseMiddleware<TSocketConnectionMiddleware>()
            where TSocketConnectionMiddleware : IConnectionMiddleware;
    }
}
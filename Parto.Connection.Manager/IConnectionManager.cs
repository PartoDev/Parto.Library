using System;
using Parto.Connection.Abstractions;

namespace Parto.Connection.Manager
{
    public interface IConnectionManager : IConnectionMiddleware
    {
        IConnectionContext Set(Guid contextId, IConnectionContext serviceContext);
        bool SetTag(Guid contextId, object tag);

        IConnectionContext? Get(Guid contextId);
        IConnectionContext? Get(object tag);
        bool Remove(Guid contextId, out IConnectionContext? serviceContext);
        bool RemoveTag(Guid contextId, out object? tag);
    }
}
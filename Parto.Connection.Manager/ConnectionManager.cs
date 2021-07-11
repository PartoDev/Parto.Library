using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Parto.Connection.Abstractions;

namespace Parto.Connection.Manager
{
    public class ConnectionManager : IConnectionManager
    {
        private readonly ConcurrentDictionary<Guid, IConnectionContext> _clients = new();
        private readonly ConcurrentDictionary<Guid, object> _tags = new();


        public IConnectionContext Set(Guid contextId, IConnectionContext serviceContext) =>
            _clients.AddOrUpdate(contextId, _ => serviceContext, (_, _) => serviceContext);

        public bool SetTag(Guid contextId, object tag) => _clients.TryGetValue(contextId, out _) &&
                                                          _tags.AddOrUpdate(contextId, _ => tag, (_, _) => tag) == tag;

        public IConnectionContext? Get(Guid contextId) =>
            _clients.TryGetValue(contextId, out var socketClient) ? socketClient : null;

        public IConnectionContext? Get(object tag)
        {
            var guid = _tags.FirstOrDefault(x => x.Value == tag)
                .Key;
            return guid == default ? null : Get(guid);
        }

        public bool Remove(Guid contextId, out IConnectionContext? serviceContext)
        {
            RemoveTag(contextId, out _);
            return _clients.TryRemove(contextId, out serviceContext);
        }

        public bool RemoveTag(Guid contextId, out object? tag) => _tags.TryRemove(contextId, out tag);

        public async ValueTask InvokeConnectionAsync(IConnectionContext context,
            NextConnectionDelegate next,
            CancellationToken cancellationToken)
        {
            Set(context.Id, context);
            await next(context, cancellationToken)
                .ConfigureAwait(false);
            Remove(context.Id, out _);
        }
    }
}
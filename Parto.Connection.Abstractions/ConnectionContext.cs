using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Parto.Connection.Abstractions
{
    public abstract class ConnectionContext : IConnectionContext
    {
        private readonly CancellationTokenSource _cancellationTokenSource;

        public ConnectionContext(IServiceProvider serviceProvider,
            params CancellationToken[] cancellationTokens)
        {
            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationTokens);
            ServiceProvider = serviceProvider;
        }

        public Guid Id { get; } = Guid.NewGuid();
        public CancellationToken CancellationToken => _cancellationTokenSource.Token;
        public IServiceProvider ServiceProvider { get; }

        public virtual void Dispose()
        {
            _cancellationTokenSource.Dispose();
            GC.SuppressFinalize(this);
        }

        public abstract ValueTask SendBlockAsync(IConnectionBlockModel block,
            CancellationToken cancellationToken = default);

        public async ValueTask<IConnectionBlockModel?> TransferBlockAsync(IConnectionBlockModel block,
            CancellationToken cancellationToken = default)
        {
            block.Guid ??= Guid.NewGuid();
            await SendBlockAsync(block, cancellationToken).ConfigureAwait(false);
            await foreach (var receiverBlock in BlockReceiverAsync(cancellationToken))
                if (receiverBlock?.Guid == block.Guid)
                    return receiverBlock;
            return null;
        }

        public async ValueTask<ConnectionBlockModel<TModel>?> TransferBlockAsync<TModel>(IConnectionBlockModel block,
            CancellationToken cancellationToken = default)
        {
            block.Guid ??= Guid.NewGuid();
            await SendBlockAsync(block, cancellationToken).ConfigureAwait(false);
            await foreach (var receiverBlock in BlockReceiverAsync<TModel>(cancellationToken))
                if (receiverBlock?.Guid == block.Guid)
                    return receiverBlock;
            return null;
        }

        public abstract IAsyncEnumerable<ConnectionBlockModel<TModel>?> BlockReceiverAsync<TModel>(
            CancellationToken cancellationToken = default);

        public abstract IAsyncEnumerable<IConnectionBlockModel?> BlockReceiverAsync(
            CancellationToken cancellationToken = default);

        public abstract ValueTask<IConnectionBlockModel?> ReceiveBlockAsync(CancellationToken cancellationToken = default);

        public abstract ValueTask<ConnectionBlockModel<TModel>?> ReceiveBlockAsync<TModel>(
            CancellationToken cancellationToken = default);
    }
}
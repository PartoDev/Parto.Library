using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Parto.Connection.Abstractions
{
    public interface IConnectionContext : IDisposable
    {
        Guid Id { get; }
        CancellationToken CancellationToken { get; }
        IServiceProvider ServiceProvider { get; }
        ValueTask SendBlockAsync(IConnectionBlockModel block, CancellationToken cancellationToken = default);

        ValueTask<IConnectionBlockModel?> TransferBlockAsync(IConnectionBlockModel block,
            CancellationToken cancellationToken = default);

        ValueTask<ConnectionBlockModel<TModel>?> TransferBlockAsync<TModel>(IConnectionBlockModel block,
            CancellationToken cancellationToken = default);

        IAsyncEnumerable<IConnectionBlockModel?> BlockReceiverAsync(CancellationToken cancellationToken = default);

        IAsyncEnumerable<ConnectionBlockModel<TModel>?> BlockReceiverAsync<TModel>(
            CancellationToken cancellationToken = default);

        ValueTask<IConnectionBlockModel?> ReceiveBlockAsync(CancellationToken cancellationToken = default);

        ValueTask<ConnectionBlockModel<TModel>?> ReceiveBlockAsync<TModel>(
            CancellationToken cancellationToken = default);
    }
}
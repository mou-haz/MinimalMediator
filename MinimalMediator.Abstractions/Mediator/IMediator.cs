using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MinimalMediator.Abstractions.Messages.Streams;

namespace MinimalMediator.Abstractions.Mediator;

public interface IMediator
{
    ValueTask<TResponse> Send<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default) where TRequest : IRequestBase<TResponse>;
    ValueTask Send<TRequest>(TRequest request, CancellationToken cancellationToken = default) where TRequest : IRequestBase;
    ValueTask<TResponse> Send<TResponse>(IRequestBase<TResponse> request, CancellationToken cancellationToken = default);

    IAsyncEnumerable<TResponse> CreateStream<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default) where TRequest : IStreamRequestBase<TResponse>;
    IAsyncEnumerable<TResponse> CreateStream<TResponse>(IStreamRequestBase<TResponse> request, CancellationToken cancellationToken = default);
}
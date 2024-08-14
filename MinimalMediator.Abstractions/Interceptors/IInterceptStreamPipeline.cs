using System.Collections.Generic;
using System.Threading;
using MinimalMediator.Abstractions.Messages.Streams;

namespace MinimalMediator.Abstractions.Interceptors;

public interface IInterceptStreamPipeline<TRequest, TResponse> /*: IStreamPipelineBehavior<TRequest,TResponse>*/
    where TRequest : IStreamRequestBase<TResponse>
{
    IAsyncEnumerable<TResponse> Handle(TRequest request, HandlerForStreamDelegate<TRequest, TResponse> next,
        CancellationToken cancellationToken);

    //IAsyncEnumerable<TResponse> IStreamPipelineBehavior<TRequest, TResponse>.Handle(TRequest request, StreamHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    //{
    //    return Handle(request, (_, _) => next(), cancellationToken);
    //}
}

public delegate IAsyncEnumerable<TResponse> HandlerForStreamDelegate<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken);

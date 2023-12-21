using System.Threading;
using System.Threading.Tasks;

namespace MinimalMediator.Abstractions.Interceptors;

public interface IInterceptPipeline<TRequest, TResponse>
    where TRequest : IRequestBase<TResponse>
{
    ValueTask<TResponse> Handle(TRequest request, HandlerDelegate<TRequest, TResponse> next,
        CancellationToken cancellationToken);
}

public interface IInterceptPipeline<TRequest>
    where TRequest : IRequestBase
{
    ValueTask Handle(TRequest request, HandlerDelegate<TRequest> next, CancellationToken cancellationToken);
}
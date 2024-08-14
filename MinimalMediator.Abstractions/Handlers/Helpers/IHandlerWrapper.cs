using System.Threading;
using System.Threading.Tasks;

namespace MinimalMediator.Abstractions.Handlers.Helpers;

public interface IHandlerWrapper<in TRequest> where TRequest : IRequestBase
{
    ValueTask Handle(TRequest request, CancellationToken cancellationToken = default);
}

public interface IHandlerWrapper<in TRequest, TResponse> where TRequest : IRequestBase<TResponse>
{
    ValueTask<TResponse> Handle(TRequest request, CancellationToken cancellationToken = default);
}
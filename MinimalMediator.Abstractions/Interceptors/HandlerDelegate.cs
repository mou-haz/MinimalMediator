using System.Threading;
using System.Threading.Tasks;

namespace MinimalMediator.Abstractions.Interceptors;

public delegate ValueTask HandlerDelegate<in TRequest>(TRequest request, CancellationToken cancellationToken);

public delegate ValueTask<TResponse> HandlerDelegate<in TRequest, TResponse>(TRequest request,
    CancellationToken cancellationToken);
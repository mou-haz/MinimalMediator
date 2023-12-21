using System.Threading;
using System.Threading.Tasks;

namespace MinimalMediator.Abstractions.Handlers;

/// <summary>
/// Base interface, for all Request handlers.
/// </summary>
/// <typeparam name="TRequest">Type of the Request, that will be handled.</typeparam>
public interface IRequestHandlerBase<in TRequest>
    where TRequest : IRequestBase
{
    ValueTask Handle(TRequest request, CancellationToken cancellationToken);
}

/// <summary>
/// <inheritdoc cref="IRequestHandlerBase{TRequest}"/>
/// </summary>
/// <typeparam name="TRequest"><inheritdoc cref="IRequestHandlerBase{TRequest}" path="/typeparam"/></typeparam>
/// <typeparam name="TResult">Type of the object, that will be returned as Request execution result.</typeparam>
public interface IRequestHandlerBase<in TRequest, TResult>
    where TRequest : IRequestBase<TResult>
{
    ValueTask<TResult> Handle(TRequest request, CancellationToken cancellationToken);
}
using System.Threading;
using System.Threading.Tasks;
using MinimalMediator.Abstractions.Messages;

namespace MinimalMediator.Abstractions.Handlers.Helpers;

internal interface IRequestHandler
{
    ValueTask Handle(IIdentifiableRequest request, CancellationToken cancellationToken);
}

internal interface IRequestHandlerWithReturn<TReturn>
{
    ValueTask<TReturn> Handle(IRequestBase<TReturn> request, CancellationToken cancellationToken);
}

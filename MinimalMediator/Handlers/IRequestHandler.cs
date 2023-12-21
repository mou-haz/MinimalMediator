namespace MinimalMediator.Handlers;

internal interface IRequestHandler
{
    ValueTask Handle(IIdentifiableRequest request, CancellationToken cancellationToken);
}

internal interface IRequestHandlerWithReturn<TReturn>
{
    ValueTask<TReturn> Handle(IRequestBase<TReturn> request, CancellationToken cancellationToken);
}
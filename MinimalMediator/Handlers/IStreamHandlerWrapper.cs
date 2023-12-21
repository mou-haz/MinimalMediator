namespace MinimalMediator.Handlers;

internal interface IStreamHandlerWrapper
{
    IAsyncEnumerable<object> Handle(IIdentifiableRequest request, CancellationToken cancellationToken);
}
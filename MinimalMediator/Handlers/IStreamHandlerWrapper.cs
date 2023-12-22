namespace MinimalMediator.Handlers;

internal interface IStreamHandlerWrapper<out TResponse>
{
    IAsyncEnumerable<TResponse> Handle(IIdentifiableRequest request, CancellationToken cancellationToken);
}
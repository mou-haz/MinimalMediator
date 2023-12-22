namespace MinimalMediator.Handlers;

internal interface IRequestHandler
{
    ValueTask Handle(IIdentifiableRequest request, CancellationToken cancellationToken);
}

public interface IRequestHandlerWithReturn<TReturn>
{
    ValueTask<TReturn> Handle(IRequestBase<TReturn> request, CancellationToken cancellationToken);
}
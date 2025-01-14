using MinimalMediator.Helpers;

namespace MinimalMediator.Handlers;

public sealed class HandlerWrapper<TRequest> : IRequestHandler, IHandlerWrapper<TRequest> where TRequest : IRequestBase
{
    private readonly HandlerDelegate<TRequest> _finalHandler;

    public HandlerWrapper(IRequestHandlerBase<TRequest> requestHandler, IEnumerable<IInterceptPipeline<TRequest>> pipelineBehaviors)
    {
        _finalHandler = requestHandler.Handle;

        foreach (var pipeline in pipelineBehaviors.Reverse())
        {
            var tempHandler = _finalHandler;
            _finalHandler = (_request, _cancellationToken) => pipeline.Handle(_request, (request, cancellationToken) => tempHandler(request, cancellationToken), _cancellationToken);
        }
    }

    public ValueTask Handle(TRequest request, CancellationToken cancellationToken = default)
    {
        request.ThrowIfNull(nameof(request));
        return _finalHandler(request, cancellationToken);
    }

    ValueTask IRequestHandler.Handle(IIdentifiableRequest request, CancellationToken cancellationToken) =>
        Handle((TRequest)request, cancellationToken);
}

public sealed class HandlerWrapper<TRequest, TResponse> : IRequestHandler, IRequestHandlerWithReturn<TResponse>, IHandlerWrapper<TRequest, TResponse> where TRequest : IRequestBase<TResponse>
{
    private readonly HandlerDelegate<TRequest, TResponse> _finalHandler;

    public HandlerWrapper(IRequestHandlerBase<TRequest, TResponse> requestHandler, IEnumerable<IInterceptPipeline<TRequest, TResponse>> pipelineBehaviors)
    {
        _finalHandler = requestHandler.Handle;

        foreach (var pipeline in pipelineBehaviors.Reverse())
        {
            var tempHandler = _finalHandler;
            _finalHandler = (_request, _cancellationToken) => pipeline.Handle(_request, (request, cancellationToken) => tempHandler(request, cancellationToken), _cancellationToken);
        }
    }

    public ValueTask<TResponse> Handle(TRequest request, CancellationToken cancellationToken = default)
    {
        request.ThrowIfNull(nameof(request));

        return _finalHandler(request, cancellationToken);
    }

    ValueTask<TResponse> IRequestHandlerWithReturn<TResponse>.Handle(IRequestBase<TResponse> request, CancellationToken cancellationToken) =>
        Handle((TRequest)request, cancellationToken);

    ValueTask IRequestHandler.Handle(IIdentifiableRequest request, CancellationToken cancellationToken)
    {
        var valueTask = Handle((TRequest)request, cancellationToken);

        return valueTask.IsCompletedSuccessfully
            ? default
            : new ValueTask(valueTask.AsTask());
    }
}
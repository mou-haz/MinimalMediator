using MinimalMediator.Helpers;

namespace MinimalMediator.Handlers;

public sealed class StreamHandlerWrapper<TRequest, TResponse> : IStreamHandlerWrapper<TResponse>, IStreamHandlerWrapper<TRequest, TResponse> where TRequest : IStreamRequestBase<TResponse>
{
    private readonly HandlerForStreamDelegate<TRequest, TResponse> _rootHandler;

    public StreamHandlerWrapper(IStreamHandler<TRequest, TResponse> requestHandler, IEnumerable<IInterceptStreamPipeline<TRequest, TResponse>> pipelineBehaviors)
    {
        var handler = (HandlerForStreamDelegate<TRequest, TResponse>)requestHandler.Handle;

        foreach (var pipeline in pipelineBehaviors.Reverse())
        {
            var handlerCopy = handler;
            var pipelineCopy = pipeline;

            handler = (_request, _cancellationToken) => pipelineCopy.Handle(_request, (request, cancellationToken) => handlerCopy(request, cancellationToken), _cancellationToken);
        }

        _rootHandler = handler;
    }

    public IAsyncEnumerable<TResponse> Handle(TRequest request, CancellationToken cancellationToken)
    {
        request.ThrowIfNull(nameof(request));

        return _rootHandler(request, cancellationToken);
    }

    IAsyncEnumerable<TResponse> IStreamHandlerWrapper<TResponse>.Handle(IIdentifiableRequest request, CancellationToken cancellationToken)
    {
        return Handle((TRequest)request, cancellationToken);
    }
}
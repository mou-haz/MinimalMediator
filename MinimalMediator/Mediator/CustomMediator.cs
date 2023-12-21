using System.Collections.Concurrent;
using MinimalMediator.Helpers;

namespace MinimalMediator.Mediator;

//TOOD: Compare with mediatR, mediator, messagePipe
//this is far better than mediatR,
//similar to mediator in perf, no code generation thou, also no regression after large number of requests
//message pipe doesn't have pipeline interception
/* full call means calling _serviceProvider.Get then handle, the other is just calling handle/send
|                               Method |        Mean |     Error |    StdDev | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
|------------------------------------- |------------:|----------:|----------:|------:|--------:|-----:|----------:|------------:|
|           SendRequest_HandlerWrapper |    42.87 ns |  0.763 ns |  0.714 ns |  0.87 |    0.02 |    1 |         - |        0.00 |
|                 SendRequest_Baseline |    49.22 ns |  0.687 ns |  0.609 ns |  1.00 |    0.00 |    2 |      64 B |        1.00 |
|                SendRequest_IMediator |    59.97 ns |  1.107 ns |  1.036 ns |  1.22 |    0.02 |    3 |         - |        0.00 |
|  SendRequest_HandlerWrapper_FullCall |    72.22 ns |  1.330 ns |  1.244 ns |  1.47 |    0.03 |    4 |         - |        0.00 |
|          SendRequest_ICustomMediator |    95.57 ns |  1.774 ns |  1.821 ns |  1.94 |    0.04 |    5 |         - |        0.00 |
| SendRequest_ICustomMediator_FullCall |   119.06 ns |  2.280 ns |  2.133 ns |  2.42 |    0.06 |    6 |         - |        0.00 |
|        SendRequest_Baseline_FullCall | 1,756.06 ns | 29.186 ns | 27.300 ns | 35.73 |    0.85 |    7 |    1304 B |       20.38 |
|       SendRequest_IMediator_FullCall | 2,003.54 ns | 15.105 ns | 13.390 ns | 40.71 |    0.50 |    8 |     960 B |       15.00 |*/
public sealed class CustomMediator : IMediator
{
    private static readonly ConcurrentDictionary<Type, object> RequestHandlers = new();
    private readonly IServiceProvider _serviceProvider;

    public CustomMediator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public ValueTask<TResponse> Send<TRequest, TResponse>(TRequest request,
        CancellationToken cancellationToken = default) where TRequest : IRequestBase<TResponse>
    {
        return InternalGetOrAddHandler<HandlerWrapper<TRequest, TResponse>>(request.GetType())
            .Handle(request, cancellationToken);
    }

    public ValueTask<TResponse> Send<TResponse>(IRequestBase<TResponse> request,
        CancellationToken cancellationToken = default)
    {
        //using the internal call here cause poor performance
        request.ThrowIfNull(nameof(request));

        if (RequestHandlers.TryGetValue(request.GetType(), out var handler))
        {
            return ((IRequestHandlerWithReturn<TResponse>)handler).Handle(request, cancellationToken);
        }

        handler = _serviceProvider.GetService(
            typeof(HandlerWrapper<,>).MakeGenericType(request.GetType(), typeof(TResponse)));

        handler.ThrowIfNull(nameof(handler));

        RequestHandlers.TryAdd(request.GetType(), handler);

        return ((IRequestHandlerWithReturn<TResponse>)handler).Handle(request, cancellationToken);
    }

    public ValueTask Send<TRequest>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : IRequestBase
    {
        return InternalGetOrAddHandler<HandlerWrapper<TRequest>>(request.GetType())
            .Handle(request, cancellationToken);
    }

    public IAsyncEnumerable<TResponse> CreateStream<TRequest, TResponse>(TRequest request,
        CancellationToken cancellationToken = default) where TRequest : IStreamRequestBase<TResponse>
    {
        return InternalGetOrAddHandler<StreamHandlerWrapper<TRequest, TResponse>>(request.GetType())
            .Handle(request, cancellationToken);
    }

    private THandler InternalGetOrAddHandler<THandler>(Type requestType,Type? handlerType = null)
    {
        requestType.ThrowIfNull(nameof(requestType));

        if (RequestHandlers.TryGetValue(requestType, out var handler))
        {
            return (THandler)handler;
        }

        handler = handlerType is null
            ? _serviceProvider.GetService(typeof(THandler))
            : (THandler)_serviceProvider.GetService(handlerType);

        handler.ThrowIfNull(nameof(handler));

        RequestHandlers.TryAdd(requestType, handler);

        return (THandler)handler;
    }
}
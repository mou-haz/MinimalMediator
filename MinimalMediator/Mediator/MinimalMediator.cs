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
|          SendRequest_IMinimalMediator |    95.57 ns |  1.774 ns |  1.821 ns |  1.94 |    0.04 |    5 |         - |        0.00 |
| SendRequest_IMinimalMediator_FullCall |   119.06 ns |  2.280 ns |  2.133 ns |  2.42 |    0.06 |    6 |         - |        0.00 |
|        SendRequest_Baseline_FullCall | 1,756.06 ns | 29.186 ns | 27.300 ns | 35.73 |    0.85 |    7 |    1304 B |       20.38 |
|       SendRequest_IMediator_FullCall | 2,003.54 ns | 15.105 ns | 13.390 ns | 40.71 |    0.50 |    8 |     960 B |       15.00 |

|                                                Method |        Mean |     Error |    StdDev | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
|------------------------------------------------------ |------------:|----------:|----------:|------:|--------:|-----:|----------:|------------:|
|                                 SendRequest_IMediator |    34.85 ns |  0.376 ns |  0.352 ns |  0.85 |    0.02 |    1 |         - |        0.00 |
|                               SendRequest_LazyWrapper |    37.11 ns |  0.437 ns |  0.409 ns |  0.90 |    0.01 |    2 |         - |        0.00 |
|                                  SendRequest_Baseline |    41.08 ns |  0.498 ns |  0.466 ns |  1.00 |    0.00 |    3 |      64 B |        1.00 |
|                 SendRequest_CustomMediator_SendSecond |    49.57 ns |  0.497 ns |  0.465 ns |  1.21 |    0.02 |    4 |         - |        0.00 |
|                           SendRequest_ICustomMediator |    55.55 ns |  0.804 ns |  0.752 ns |  1.35 |    0.03 |    5 |         - |        0.00 |
| SendRequest_NewCustomMediator_SendWithNoGenericParams |    58.61 ns |  0.950 ns |  0.842 ns |  1.43 |    0.03 |    6 |         - |        0.00 |
|                      SendRequest_LazyWrapper_FullCall |    62.42 ns |  0.745 ns |  0.660 ns |  1.52 |    0.03 |    7 |         - |        0.00 |
|   SendRequest_ICustomMediator_SendWithNoGenericParams |    63.05 ns |  0.833 ns |  0.780 ns |  1.54 |    0.03 |    7 |         - |        0.00 |
|                         SendRequest_NewCustomMediator |    63.71 ns |  0.946 ns |  0.790 ns |  1.55 |    0.03 |    7 |         - |        0.00 |
|                  SendRequest_ICustomMediator_FullCall |    76.96 ns |  1.108 ns |  1.036 ns |  1.87 |    0.04 |    8 |         - |        0.00 |
|                SendRequest_NewCustomMediator_FullCall |    81.91 ns |  0.836 ns |  0.741 ns |  2.00 |    0.02 |    9 |         - |        0.00 |
|                         SendRequest_Baseline_FullCall | 1,050.38 ns | 18.477 ns | 17.284 ns | 25.57 |    0.48 |   10 |    1304 B |       20.38 |
|                        SendRequest_IMediator_FullCall | 1,413.14 ns | 22.516 ns | 21.061 ns | 34.41 |    0.68 |   11 |     960 B |       15.00 |

|                                                Method |        Mean |     Error |    StdDev | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
|------------------------------------------------------ |------------:|----------:|----------:|------:|--------:|-----:|----------:|------------:|
|                                 SendRequest_IMediator |    41.13 ns |  0.381 ns |  0.338 ns |  0.86 |    0.01 |    1 |         - |        0.00 |
|                                  SendRequest_Baseline |    47.97 ns |  0.703 ns |  0.624 ns |  1.00 |    0.00 |    2 |      64 B |        1.00 |
|                            SendRequest_HandlerWrapper |    71.67 ns |  1.522 ns |  1.563 ns |  1.49 |    0.04 |    3 |         - |        0.00 |
|                               SendRequest_LazyWrapper |    72.99 ns |  1.540 ns |  1.286 ns |  1.52 |    0.04 |    3 |         - |        0.00 |
|                           SendRequest_MinimalMediator |    93.22 ns |  1.807 ns |  1.690 ns |  1.94 |    0.05 |    4 |         - |        0.00 |
|                         SendRequest_NewCustomMediator |    94.15 ns |  1.953 ns |  1.918 ns |  1.97 |    0.05 |    4 |         - |        0.00 |
|                   SendRequest_HandlerWrapper_FullCall |    96.32 ns |  1.907 ns |  2.040 ns |  2.01 |    0.06 |    4 |         - |        0.00 |
|                      SendRequest_LazyWrapper_FullCall |   100.98 ns |  1.435 ns |  1.343 ns |  2.11 |    0.04 |    5 |         - |        0.00 |
|   SendRequest_MinimalMediator_SendWithNoGenericParams |   104.16 ns |  1.674 ns |  1.398 ns |  2.17 |    0.03 |    6 |         - |        0.00 |
|                  SendRequest_MinimalMediator_FullCall |   117.41 ns |  2.163 ns |  1.918 ns |  2.45 |    0.05 |    7 |         - |        0.00 |
|                SendRequest_NewCustomMediator_FullCall |   127.69 ns |  2.123 ns |  1.986 ns |  2.66 |    0.07 |    8 |         - |        0.00 |
| SendRequest_NewCustomMediator_SendWithNoGenericParams |   130.47 ns |  2.610 ns |  2.792 ns |  2.72 |    0.07 |    8 |         - |        0.00 |
|                         SendRequest_Baseline_FullCall | 1,371.82 ns | 20.172 ns | 16.844 ns | 28.59 |    0.41 |    9 |    1384 B |       21.62 |
|                                   SendRequest_MediatR | 1,574.17 ns | 19.975 ns | 15.595 ns | 32.79 |    0.46 |   10 |     888 B |       13.88 |
|                        SendRequest_IMediator_FullCall | 1,695.19 ns | 32.575 ns | 43.487 ns | 35.40 |    1.09 |   11 |     960 B |       15.00 |
|                          SendRequest_MediatR_FullCall | 1,840.87 ns | 21.769 ns | 19.297 ns | 38.38 |    0.60 |   12 |     960 B |       15.00 |
*/
public sealed class MinimalMediator(IServiceProvider serviceProvider) : IMediator
{
    public ValueTask<TResponse> Send<TRequest, TResponse>(TRequest request,
        CancellationToken cancellationToken = default) where TRequest : IRequestBase<TResponse> =>
        InternalGetOrAddHandler<HandlerWrapper<TRequest, TResponse>>(request.GetType())
            .Handle(request, cancellationToken);

    public ValueTask<TResponse> Send<TResponse>(IRequestBase<TResponse> request,
        CancellationToken cancellationToken = default)
    {
        //using the internal call here cause poor performance
        request.ThrowIfNull(nameof(request));

        var handler = serviceProvider.GetRequiredService(
            typeof(HandlerWrapper<,>).MakeGenericType(request.GetType(), typeof(TResponse)));

        handler.ThrowIfNull(nameof(handler)
#if DEBUG
            , $"[{nameof(MinimalMediator)}]: Couldn't find handler for request of type {request.GetType().Name}"
#endif
        );

        return ((IRequestHandlerWithReturn<TResponse>)handler).Handle(request, cancellationToken);
    }

    public ValueTask Send<TRequest>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : IRequestBase =>
        InternalGetOrAddHandler<HandlerWrapper<TRequest>>(request.GetType())
            .Handle(request, cancellationToken);

    public IAsyncEnumerable<TResponse> CreateStream<TRequest, TResponse>(TRequest request,
        CancellationToken cancellationToken = default) where TRequest : IStreamRequestBase<TResponse> =>
        InternalGetOrAddHandler<StreamHandlerWrapper<TRequest, TResponse>>(request.GetType())
            .Handle(request, cancellationToken);

    public IAsyncEnumerable<TResponse> CreateStream<TResponse>(IStreamRequestBase<TResponse> request,
        CancellationToken cancellationToken = default)
    {
        //using the internal call here cause poor performance
        request.ThrowIfNull(nameof(request));

        var handler = serviceProvider.GetRequiredService(
            typeof(StreamHandlerWrapper<,>).MakeGenericType(request.GetType(), typeof(TResponse)));

        handler.ThrowIfNull(nameof(handler)
#if DEBUG
            , $"[{nameof(SingletonMinimalMediator)}]: Couldn't find stream handler for request of type {request.GetType().Name}"
#endif
        );

        return ((IStreamHandlerWrapper<TResponse>)handler).Handle(request, cancellationToken);
    }

    private THandler InternalGetOrAddHandler<THandler>(Type requestType, Type? handlerType = null) where THandler : notnull
    {
        requestType.ThrowIfNull(nameof(requestType));

        var handler = handlerType is null
            ? serviceProvider.GetRequiredService<THandler>()
            : (THandler)serviceProvider.GetRequiredService(handlerType);

        handler.ThrowIfNull(nameof(handler)
#if DEBUG
            , $"[{nameof(MinimalMediator)}]: Couldn't find handler for request of type {requestType}"
#endif
        );

        return handler;
    }
}

public sealed class SingletonMinimalMediator(IServiceProvider serviceProvider) : IMediator
{
    private static readonly ConcurrentDictionary<Type, object> RequestHandlers = new();

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

        handler = serviceProvider.GetRequiredService(
            typeof(HandlerWrapper<,>).MakeGenericType(request.GetType(), typeof(TResponse)));

        handler.ThrowIfNull(nameof(handler)
#if DEBUG
            , $"[{nameof(SingletonMinimalMediator)}]: Couldn't find handler for stream of type {request.GetType().Name}"
#endif
        );

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

    public IAsyncEnumerable<TResponse> CreateStream<TResponse>(IStreamRequestBase<TResponse> request,
        CancellationToken cancellationToken = default)
    {
        //using the internal call here cause poor performance
        request.ThrowIfNull(nameof(request));

        if (RequestHandlers.TryGetValue(request.GetType(), out var handler))
        {
            return ((IStreamHandlerWrapper<TResponse>)handler).Handle(request, cancellationToken);
        }

        handler = serviceProvider.GetRequiredService(
            typeof(StreamHandlerWrapper<,>).MakeGenericType(request.GetType(), typeof(TResponse)));

        handler.ThrowIfNull(nameof(handler)
#if DEBUG
            , $"[{nameof(SingletonMinimalMediator)}]: Couldn't find stream handler for stream of type {request.GetType().Name}"
#endif
        );

        RequestHandlers.TryAdd(request.GetType(), handler);
        return ((IStreamHandlerWrapper<TResponse>)handler).Handle(request, cancellationToken);
    }

    private THandler InternalGetOrAddHandler<THandler>(Type requestType, Type? handlerType = null)
        where THandler : notnull
    {
        requestType.ThrowIfNull(nameof(requestType));

        if (RequestHandlers.TryGetValue(requestType, out var handler))
        {
            return (THandler)handler;
        }

        handler = handlerType is null
            ? serviceProvider.GetRequiredService<THandler>()
            : (THandler)serviceProvider.GetRequiredService(handlerType);

        handler.ThrowIfNull(nameof(handler)
            #if DEBUG
            , $"[{nameof(SingletonMinimalMediator)}]: Couldn't find handler for request of type {requestType}"
            #endif
            );

        RequestHandlers.TryAdd(requestType, handler);

        return (THandler)handler;
    }
}
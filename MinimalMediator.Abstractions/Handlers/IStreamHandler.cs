using System.Collections.Generic;
using System.Threading;
using MinimalMediator.Abstractions.Messages.Streams;

namespace MinimalMediator.Abstractions.Handlers;

/// <summary>
/// Base interface, for all Stream handlers.
/// </summary>
/// <typeparam name="TStreamRequest">Type of the Stream, that will be handled.</typeparam>
/// <typeparam name="TResult">Type of the object, that will be returned as Stream execution result.</typeparam>
public interface IStreamHandler<in TStreamRequest, out TResult>
    where TStreamRequest : IStreamRequestBase<TResult>
{
    IAsyncEnumerable<TResult> Handle(TStreamRequest request, CancellationToken cancellationToken);
}
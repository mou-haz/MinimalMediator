using System.Collections.Generic;

namespace MinimalMediator.Abstractions.Messages.Streams;

/// <summary>
/// Represents Query for an <see cref="IAsyncEnumerable{T}"/>
/// </summary>
/// <typeparam name="TResult">Type of the object, that will be returned as the stream result.</typeparam>
public interface IStreamRequestBase<out TResult> : IIdentifiableRequest
{
}
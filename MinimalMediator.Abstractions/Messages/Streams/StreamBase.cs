namespace MinimalMediator.Abstractions.Messages.Streams;

/// <summary>
/// Abstract record, that encapsulate functionality of <see cref="IStreamRequestBase{TResult}"/> and adding request identity.
/// </summary>
/// <typeparam name="TResult"><inheritdoc cref="RequestBase" path="/typeparam"/></typeparam>
public abstract record StreamBase<TResult> : RequestBase<TResult>, IStreamRequestBase<TResult>;
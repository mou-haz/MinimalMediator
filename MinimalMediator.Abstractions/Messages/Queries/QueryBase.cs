namespace MinimalMediator.Abstractions.Messages.Queries;

/// <summary>
/// Abstract record, that encapsulate functionality of <see cref="IQuery{TResult}"/> and adding request identity.
/// </summary>
/// <typeparam name="TResult"><inheritdoc cref="RequestBase" path="/typeparam"/></typeparam>
public abstract record QueryBase<TResult> : RequestBase<TResult>, IQuery<TResult>;
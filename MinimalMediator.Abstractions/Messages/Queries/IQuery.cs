namespace MinimalMediator.Abstractions.Messages.Queries;

/// <summary>
/// Represents Query functionality in CQRS architecture approach.
/// </summary>
/// <typeparam name="TResult">Type of the object, that will be returned as the command execution result.</typeparam>
public interface IQuery<out TResult> : IRequestBase<TResult>
{
}
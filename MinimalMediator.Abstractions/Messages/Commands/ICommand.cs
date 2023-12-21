namespace MinimalMediator.Abstractions.Messages.Commands;

/// <summary>
/// Represents Command functionality in CQRS architecture approach.
/// <para><see href="https://docs.microsoft.com/en-us/azure/architecture/patterns/cqrs">Read more about CQRS</see>.</para>
/// </summary>
public interface ICommand : IRequestBase
{
}

/// <summary>
/// <inheritdoc cref="ICommand"/>
/// <para><typeparamref name="TResult"/> is the type of the object, that will be returned as the command execution result.</para>
/// </summary>
/// <typeparam name="TResult">Type of the object, that will be returned as the command execution result.</typeparam>
public interface ICommand<out TResult> : IRequestBase<TResult>
{
}
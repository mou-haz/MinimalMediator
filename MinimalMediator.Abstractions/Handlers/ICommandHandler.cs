using MinimalMediator.Abstractions.Messages.Commands;

namespace MinimalMediator.Abstractions.Handlers;

/// <summary>
/// Base interface, for all command handlers.
/// </summary>
/// <typeparam name="TCommand">Type of the command, that will be handled.</typeparam>
public interface ICommandHandler<in TCommand> : IRequestHandlerBase<TCommand>
    where TCommand : ICommand
{
}

/// <summary>
/// <inheritdoc cref="ICommandHandler{TCommand}"/>
/// </summary>
/// <typeparam name="TCommand"><inheritdoc cref="ICommandHandler{TCommand}" path="/typeparam"/></typeparam>
/// <typeparam name="TResult">Type of the object, that will be returned as command execution result.</typeparam>
public interface ICommandHandler<in TCommand, TResult> : IRequestHandlerBase<TCommand, TResult>
    where TCommand : ICommand<TResult>
{
}
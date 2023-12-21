namespace MinimalMediator.Abstractions.Messages.Commands;

/// <summary>
/// Abstract record, that encapsulate functionality of <see cref="ICommand"/> that change state of system and adding request identity.
/// </summary>
public abstract record CommandBase : RequestBase, ICommand;

/// <summary>
/// Abstract record, that encapsulate functionality of <see cref="ICommand"/> that change state of system and adding request identity.
/// </summary>
/// <typeparam name="TResult"><inheritdoc cref="RequestBase{TResult}" path="/typeparam"/></typeparam>
public abstract record CommandBase<TResult> : RequestBase<TResult>, ICommand<TResult>;

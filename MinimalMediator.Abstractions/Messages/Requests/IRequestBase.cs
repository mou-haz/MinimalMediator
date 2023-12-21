namespace MinimalMediator.Abstractions.Messages.Requests;

public interface IRequestBase : IIdentifiableRequest
{
}

public interface IRequestBase<out TReturn> : IIdentifiableRequest
{
}
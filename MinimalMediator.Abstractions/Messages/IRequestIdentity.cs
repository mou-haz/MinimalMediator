using System;

namespace MinimalMediator.Abstractions.Messages;

/// <summary>
/// Adds <see cref="Guid"/> identifier for request.
/// </summary>
public interface IIdentifiableRequest
{
    /// <summary>
    /// <see cref="Guid"/> identifier of request.
    /// </summary>
    Guid RequestId
    {
        get;
    }
}
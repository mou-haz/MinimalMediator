using System;

namespace MinimalMediator.Abstractions.LazyHelpers;

public sealed class LazyRequestHandler<T> : Lazy<T>, ILazyProvider
{
    public LazyRequestHandler(IServiceProvider serviceProvider)
        : base(() => (T)serviceProvider.GetService(typeof(T)))
    {
    }

    object? ILazyProvider.Value => Value;
}
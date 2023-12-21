namespace MinimalMediator.Abstractions.LazyHelpers;

public interface ILazyProvider
{
    object? Value
    {
        get;
    }
}
namespace MinimalMediator.Helpers;

internal static class ExceptionHelpers
{
    public static void ThrowIfNull(this object? value, string paramName, string? message = null)
    {
        if (value is null)
        {
            throw new ArgumentNullException(paramName, message);
        }
    }
}
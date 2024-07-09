using System.Runtime.CompilerServices;
using Ardalis.GuardClauses;

namespace CQRS.Core.Extensions.GuardClauses;

public static class EnsureGuardExtensions
{
    public static void Valid(
        this ICustomGuardClauses _,
        [System.Diagnostics.CodeAnalysis.NotNull] [ValidatedNotNull] bool? input,
        string? message = null)
    {
        if (input is true)
        {
            return;
        }

        message ??= "The conditions for a valid operation were not met.";

        throw new InvalidOperationException(message);
    }
    
    public static void False(
        this ICustomGuardClauses _,
        [ValidatedNotNull] bool? input,
        string? message = null,
        [CallerArgumentExpression("input")] string? variableName = null)
    {
        if (input is null or false)
        {
            return;
        }

        message ??= $"The value of '{variableName}' must be 'false'.";

        throw new InvalidOperationException(message);
    }
    
    public static void True(
        this ICustomGuardClauses guardClause,
        [System.Diagnostics.CodeAnalysis.NotNull] [ValidatedNotNull] bool? input,
        string? message = null,
        [CallerArgumentExpression("input")] string? variableName = null)
    {
        message ??= $"The value of '{variableName}' must be 'true'.";
        GuardExtension.Require.Valid(input, message);
    }
}
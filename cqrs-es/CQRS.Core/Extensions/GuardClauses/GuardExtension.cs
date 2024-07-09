using Ardalis.GuardClauses;

namespace CQRS.Core.Extensions.GuardClauses;

public sealed record GuardExtension : ICustomGuardClauses
{
    
    internal static ICustomGuardClauses Require => new GuardExtension();
    
    public static GuardExtension Ensure => new GuardExtension();

}
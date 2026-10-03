namespace ElectricDashboardApi.Models.Options;

/// <summary>
/// Per-account login lockout settings, bound from the "LoginLockout" configuration section.
/// </summary>
public class LoginLockoutOptions
{
    /// <summary>Number of consecutive failed logins (per account + client IP) before a temporary lockout.</summary>
    public int MaxFailures { get; init; } = 5;

    /// <summary>How long the account stays locked out (in minutes) once the failure threshold is reached.</summary>
    public int LockoutMinutes { get; init; } = 5;
}

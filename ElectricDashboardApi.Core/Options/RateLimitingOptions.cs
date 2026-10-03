namespace ElectricDashboardApi.Models.Options;

/// <summary>
/// Per-endpoint rate limiting windows, bound from the "RateLimiting" configuration section.
/// </summary>
public class RateLimitingOptions
{
    public RateLimitingEndpointOptions Login { get; init; } = new();
    public RateLimitingEndpointOptions Register { get; init; } = new();
    public RateLimitingEndpointOptions RefreshToken { get; init; } = new();
}

public class RateLimitingEndpointOptions
{
    /// <summary>Length of the window (in seconds) during which <see cref="PermitLimit"/> requests are allowed per client IP.</summary>
    public int WindowSeconds { get; init; } = 60;

    /// <summary>Maximum number of requests allowed per window per client IP.</summary>
    public int PermitLimit { get; init; } = 5;
}

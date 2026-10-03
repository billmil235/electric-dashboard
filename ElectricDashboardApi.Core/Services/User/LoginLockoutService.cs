using System.Collections.Concurrent;
using ElectricDashboardApi.Models.Options;
using Microsoft.Extensions.Options;

namespace ElectricDashboard.Services.User;

/// <summary>
/// Tracks consecutive failed login attempts per (username, client IP) pair and temporarily
/// locks the pair out after <see cref="LoginLockoutOptions.MaxFailures"/> consecutive failures.
/// </summary>
/// <remarks>
/// This is an in-memory implementation, which is correct for a single API instance.
/// For multiple API instances behind a load balancer, swap the storage for a shared
/// store (e.g. Redis) or rely on Keycloak's built-in brute-force protection instead.
/// </remarks>
public interface ILoginLockoutService
{
    /// <summary>Returns <c>true</c> if the account/IP pair is currently locked out.</summary>
    bool IsLockedOut(string username, string? ipAddress, out TimeSpan retryAfter);

    /// <summary>Records a failed login attempt and returns the number of attempts remaining (0 = now locked out).</summary>
    int RegisterFailure(string username, string? ipAddress);

    /// <summary>Clears the failure count after a successful login.</summary>
    void ClearFailures(string username, string? ipAddress);
}

public class LoginLockoutService(IOptions<LoginLockoutOptions> options) : ILoginLockoutService
{
    private readonly LoginLockoutOptions _options = options.Value;
    private readonly ConcurrentDictionary<string, LockoutEntry> _entries = new(StringComparer.OrdinalIgnoreCase);
    private readonly object _pruneGate = new();
    private DateTime _lastPruneUtc = DateTime.MinValue;

    private sealed class LockoutEntry
    {
        public int FailureCount;
        public DateTime? LockoutUntilUtc;
        public DateTime LastActivityUtc = DateTime.UtcNow;
    }

    public bool IsLockedOut(string username, string? ipAddress, out TimeSpan retryAfter)
    {
        if (_entries.TryGetValue(BuildKey(username, ipAddress), out var entry))
        {
            lock (entry)
            {
                if (entry.LockoutUntilUtc is { } until && until > DateTime.UtcNow)
                {
                    retryAfter = until - DateTime.UtcNow;
                    return true;
                }

                if (entry.LockoutUntilUtc.HasValue)
                {
                    // Lockout expired — reset the counter so the user gets a fresh set of attempts.
                    entry.LockoutUntilUtc = null;
                    entry.FailureCount = 0;
                }
            }
        }

        retryAfter = TimeSpan.Zero;
        return false;
    }

    public int RegisterFailure(string username, string? ipAddress)
    {
        PruneStaleEntries();

        var entry = _entries.GetOrAdd(BuildKey(username, ipAddress), _ => new LockoutEntry());

        lock (entry)
        {
            entry.LastActivityUtc = DateTime.UtcNow;
            entry.FailureCount++;

            if (entry.FailureCount >= _options.MaxFailures)
            {
                entry.LockoutUntilUtc = DateTime.UtcNow + LockoutDuration;
            }

            return Math.Max(0, _options.MaxFailures - entry.FailureCount);
        }
    }

    public void ClearFailures(string username, string? ipAddress)
    {
        _entries.TryRemove(BuildKey(username, ipAddress), out _);
    }

    private TimeSpan LockoutDuration => TimeSpan.FromMinutes(Math.Max(1, _options.LockoutMinutes));

    private static string BuildKey(string username, string? ipAddress)
    {
        var name = string.IsNullOrWhiteSpace(username)
            ? "unknown"
            : username.Trim().ToLowerInvariant();
        var ip = string.IsNullOrWhiteSpace(ipAddress) ? "unknown-ip" : ipAddress;
        return $"{name}|{ip}";
    }

    /// <summary>Periodically removes entries that are no longer locked out and have had no recent activity, so the store cannot grow unbounded.</summary>
    private void PruneStaleEntries()
    {
        var now = DateTime.UtcNow;

        lock (_pruneGate)
        {
            if ((now - _lastPruneUtc).TotalMinutes < 5)
            {
                return;
            }

            _lastPruneUtc = now;
        }

        foreach (var (key, entry) in _entries)
        {
            bool isStale;
            lock (entry)
            {
                var lockoutActive = entry.LockoutUntilUtc is { } until && until > now;
                isStale = !lockoutActive && (now - entry.LastActivityUtc).TotalMinutes > 30;
            }

            if (isStale)
            {
                _entries.TryRemove(key, out _);
            }
        }
    }
}

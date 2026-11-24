using System.Threading.RateLimiting;

namespace jcode.web.util;

/// <summary>
/// 限流器
/// </summary>
public static class PartitionedRateLimiterHelper
{
    public static readonly PartitionedRateLimiter<string> Limiter = PartitionedRateLimiter.Create<string, string>(resource =>
    {
        return RateLimitPartition.GetConcurrencyLimiter(resource, key =>
            new ConcurrencyLimiterOptions()
            {
                PermitLimit = 10000
            }
        );
    });
}
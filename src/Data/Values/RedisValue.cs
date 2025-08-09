namespace codecrafters_redis.src.Data.Values;
public abstract class RedisValue
{
    public DateTime? Expiry { get; set; }
    public bool IsExpired => Expiry.HasValue && DateTime.UtcNow > Expiry.Value;
    public abstract string Type { get; }

    protected RedisValue(DateTime? expiry = null)
    {
        Expiry = expiry;
    }
}

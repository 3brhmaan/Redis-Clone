namespace codecrafters_redis.src;
public class RedisValue
{
    public string Value { get; set; } = string.Empty;
    public DateTime? Expiry { get; set; } = null;
    public bool IsExpired => Expiry.HasValue && DateTime.UtcNow > Expiry.Value;

    public RedisValue(string value , DateTime? expiry = null)
    {
        Value = value;
        Expiry = expiry;
    }
}

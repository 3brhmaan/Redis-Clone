namespace codecrafters_redis.src;
public class RedisValue
{
    //public string Type { get; set; } = "none";
    public string? StringValue { get; set; } = null;
    public List<string>? ListValue { get; set; } = null;
    public DateTime? Expiry { get; set; } = null;
    public bool IsExpired => Expiry.HasValue && DateTime.UtcNow > Expiry.Value;

    public RedisValue() { }
    public RedisValue(string value , DateTime? expiry = null)
    {
        StringValue = value;
        Expiry = expiry;
    }
}

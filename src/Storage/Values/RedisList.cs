namespace codecrafters_redis.src.Storage.Values;
public class RedisList : RedisValue
{
    public override string Type => RedisDataType.List;
    public List<string> Values { get; set; } = new();

    public RedisList(DateTime? expiry = null) : base(expiry) { }
}

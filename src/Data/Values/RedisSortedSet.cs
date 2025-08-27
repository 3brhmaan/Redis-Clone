namespace codecrafters_redis.src.Data.Values;
public class RedisSortedSet : RedisValue
{
    public override string Type => RedisDataType.SortedSet;
    public SortedDictionary<double , string> Set { get; set; } = new();

    public RedisSortedSet(DateTime? expiry = null) : base(expiry) { }
}

namespace codecrafters_redis.src.Storage.Values;
public class RedisSortedSet : RedisValue
{
    public override string Type => RedisDataType.SortedSet;
    public SortedSet<(double key, string value)> Set { get; set; } = new(
        Comparer<(double key, string value)>.Create((x , y) =>
        {
            int keyComparison = x.key.CompareTo(y.key);
            return keyComparison != 0 ? keyComparison : x.value.CompareTo(y.value);
        })
    );

    public RedisSortedSet(DateTime? expiry = null) : base(expiry) { }
}

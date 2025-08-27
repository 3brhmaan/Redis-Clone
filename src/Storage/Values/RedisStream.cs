namespace codecrafters_redis.src.Storage.Values;
internal class RedisStream : RedisValue
{
    public override string Type => RedisDataType.Stream;
    public List<RedisStreamEntry> Entries { get; set; } = new();

    public RedisStream(DateTime? expiry = null) : base(expiry) { }
}

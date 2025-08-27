namespace codecrafters_redis.src.Storage.Values;
public class RedisString : RedisValue
{
    public override string Type => RedisDataType.String;
    public string Value { get; set; }

    public RedisString(string value , DateTime? expiry = null) : base(expiry)
    {
        Value = value;
    }
}

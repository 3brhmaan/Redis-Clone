namespace codecrafters_redis.src.Data.Values;
public class RedisStreamEntry
{
    public string Id { get; set; }
    public Dictionary<string , string> Fields { get; set; } = new();
}

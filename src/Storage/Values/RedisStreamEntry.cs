namespace codecrafters_redis.src.Storage.Values;
public class RedisStreamEntry
{
    public string Id { get; set; }
    public Dictionary<string , string> Fields { get; set; } = new();
}

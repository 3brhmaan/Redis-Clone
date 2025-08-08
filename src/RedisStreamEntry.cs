namespace codecrafters_redis.src;
public class RedisStreamEntry
{
    public string Id { get; set; }
    public Dictionary<string , string> KeyValuePairs { get; set; } = new();
}

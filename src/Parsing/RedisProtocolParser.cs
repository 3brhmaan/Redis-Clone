namespace codecrafters_redis.src.Parsing;
public static class RedisProtocolParser
{
    public static string[] Parse(string request)
    {
        return request
            .Split("\r\n" , StringSplitOptions.RemoveEmptyEntries)
            .Where(value => !(value.StartsWith("*") && value.Trim().Length > 1 || value.StartsWith("$") && value.Trim().Length > 1))
            .Select(value => value.Trim())
            .ToArray();
    }
}

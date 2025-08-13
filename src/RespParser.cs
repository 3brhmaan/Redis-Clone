namespace codecrafters_redis.src;
public static class RespParser
{
    public static string[] Parse(string request)
    {
        return request
            .Split("\r\n" , StringSplitOptions.RemoveEmptyEntries)
            .Where(value => !((value.StartsWith("*") && value.Trim().Length > 1) || (value.StartsWith("$") && value.Trim().Length > 1)))
            .ToArray();
    }
}

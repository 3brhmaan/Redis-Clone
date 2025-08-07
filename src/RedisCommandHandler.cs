namespace codecrafters_redis.src;

public class RedisCommandHandler
{
    private readonly Dictionary<string , RedisValue> store;

    public RedisCommandHandler(Dictionary<string , RedisValue> store)
    {
        this.store = store;
    }

    public string ParseRedisCommand(string request)
    {
        var commandParts = ParseRespRequest(request);
        foreach (var kv in commandParts)
            Console.WriteLine(kv);

        var commandName = commandParts[0].ToUpper();
        var commandArguments = commandParts.Skip(1).ToArray();

        return commandName switch
        {
            "PING" => HandlePing(),
            "ECHO" => HandleEcho(commandArguments),
            "SET" => HandleSet(commandArguments),
            "GET" => HandleGet(commandArguments),
            _ => "-ERR unknown command\r\n"
        };
    }

    private string[] ParseRespRequest(string request)
    {
        var result = request
            .Split("\r\n" , StringSplitOptions.RemoveEmptyEntries)
            .Where((value) => !value.StartsWith('*') && !value.StartsWith('$'))
            .ToArray();

        return result;
    }

    private string HandlePing()
    {
        return "+PONG\r\n";
    }

    private string HandleEcho(string[] arguments)
    {
        string message = arguments[0];
        return $"+{message}\r\n";
    }

    private string HandleSet(string[] arguments)
    {
        string key = arguments[0];
        string value = arguments[1];

        var redisValue = new RedisValue(value);

        // Handle expiration (PX option)
        if (arguments.Length > 2 && arguments[2].ToLower() == "px")
        {
            Console.WriteLine($"MilliSecond: {arguments[3]}");
            if (int.TryParse(arguments[3] , out int milliseconds))
            {
                redisValue.Expiry = DateTime.UtcNow.AddMilliseconds(milliseconds);
            }
        }

        Console.WriteLine($"{key} : {value}");
        store[key] = redisValue;
        return "+OK\r\n";
    }

    private string HandleGet(string[] arguments)
    {
        string key = arguments[0];

        if (!store.ContainsKey(key))
        {
            return "$-1\r\n";
        }

        var redisValue = store[key];

        if (redisValue.IsExpired)
        {
            store.Remove(key);
            return "$-1\r\n";
        }

        return $"${redisValue.Value.Length}\r\n{redisValue.Value}\r\n";
    }
}

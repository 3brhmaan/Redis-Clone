using System.Text;

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
            "RPUSH" => HandleRpush(commandArguments),
            "LRANGE" => HandleLrange(commandArguments),
            "LPUSH" => HandleLpush(commandArguments),
            _ => "-ERR unknown command\r\n"
        };
    }

    private string HandleLrange(string[] arguments)
    {
        string key = arguments[0];
        int startIdx = int.Parse(arguments[1]);
        int endIdx = int.Parse(arguments[2]);

        if (!store.ContainsKey(key))
            return "*0\r\n";

        var value = store[key];

        if (startIdx < 0)
            startIdx = value.ListValue!.Count + startIdx;

        if(endIdx < 0)
            endIdx = value.ListValue!.Count + endIdx;

        startIdx = Math.Max(0, startIdx);
        endIdx = Math.Max(0, endIdx);

        if (endIdx >= value.ListValue!.Count)
            endIdx = value.ListValue.Count - 1;

        if (startIdx >= value.ListValue!.Count || startIdx > endIdx)
            return "*0\r\n";

        var result = new StringBuilder();
        result.Append($"*{endIdx - startIdx + 1}\r\n");

        for(int i=startIdx; i<=endIdx; i++)
            result.Append($"${value.ListValue[i].Length}\r\n{value.ListValue[i]}\r\n");

        return result.ToString();
    }

    private string HandlePush(string[] arguments , Action<RedisValue , List<string>> action)
    {
        var key = arguments[0];
        var values = arguments.Skip(1).ToList();

        var redisValue = new RedisValue();

        if (!store.ContainsKey(key))
            redisValue.ListValue = new();
        else
            redisValue = store[key];

        action(redisValue , values);

        store[key] = redisValue;

        return $":{store[key]!.ListValue!.Count}\r\n";
    }

    private string HandleLpush(string[] arguments)
    {
        return HandlePush(arguments , (redisValue , values) =>
        {
            foreach (var e in values)
                redisValue.ListValue.Insert(0 , e);
        });
    }

    private string HandleRpush(string[] arguments)
    {
        return HandlePush(arguments, (redisValue, values) => redisValue!.ListValue!.AddRange(values));
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
            //Console.WriteLine($"MilliSecond: {arguments[3]}");
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

        return $"${redisValue.StringValue.Length}\r\n{redisValue.StringValue}\r\n";
    }
}

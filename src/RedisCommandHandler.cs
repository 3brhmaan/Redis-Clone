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
            "PING" => HandlePING(),
            "ECHO" => HandleECHO(commandArguments),
            "SET" => HandleSET(commandArguments),
            "GET" => HandleGET(commandArguments),
            "RPUSH" => HandleRPUSH(commandArguments),
            "LPUSH" => HandleLPUSH(commandArguments),
            "LRANGE" => HandleLRANGE(commandArguments),
            "LLEN" => HandleLEN(commandArguments),
            "LPOP" => HandleLPOP(commandArguments),
            _ => "-ERR unknown command\r\n"
        };
    }

    private string HandleLPOP(string[] arguments)
    {
        string key = arguments[0];
        if(!store.ContainsKey(key))
            return "$-1\r\n";

        var value = store[key];
        if(value.ListValue is null ||  value.ListValue.Count == 0)
            return "$-1\r\n";

        var firstElement = value.ListValue[0];
        value.ListValue.RemoveAt(0);

        return $"${firstElement.Length}\r\n{firstElement}\r\n";
    }

    private string HandleLEN(string[] arguments)
    {
        string key = arguments[0];

        if(!store.ContainsKey(key))
            return ":0\r\n";

        return $":{store[key].ListValue?.Count}\r\n";
    }

    private string HandleLRANGE(string[] arguments)
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

    private string HandleGeneralPush(string[] arguments , Action<RedisValue , List<string>> action)
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

    private string HandleLPUSH(string[] arguments)
    {
        return HandleGeneralPush(arguments , (redisValue , values) =>
        {
            foreach (var e in values)
                redisValue.ListValue.Insert(0 , e);
        });
    }

    private string HandleRPUSH(string[] arguments)
    {
        return HandleGeneralPush(arguments, (redisValue, values) => redisValue!.ListValue!.AddRange(values));
    }

    private string HandlePING()
    {
        return "+PONG\r\n";
    }

    private string HandleECHO(string[] arguments)
    {
        string message = arguments[0];
        return $"+{message}\r\n";
    }

    private string HandleSET(string[] arguments)
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

    private string HandleGET(string[] arguments)
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

    private string[] ParseRespRequest(string request)
    {
        var result = request
            .Split("\r\n" , StringSplitOptions.RemoveEmptyEntries)
            .Where((value) => !value.StartsWith('*') && !value.StartsWith('$'))
            .ToArray();

        return result;
    }
}

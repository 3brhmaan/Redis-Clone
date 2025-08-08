using System.Collections.Concurrent;
using System.Text;

namespace codecrafters_redis.src;


public class RedisCommandHandler
{
    private readonly ConcurrentDictionary<string , RedisValue> store;
    private readonly Dictionary<string , object> keyLocks;

    public RedisCommandHandler()
    {
        store = new();
        keyLocks = new();
    }

    public string ParseRedisCommand(string request)
    {
        var commandParts = ParseRespRequest(request);
        
        //foreach (var part in commandParts)
        //    Console.Write($"{Thread.CurrentThread.ManagedThreadId}: {part} ");
        //Console.WriteLine();

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
            "BLPOP" => HandleBLPOP(commandArguments),
            _ => "-ERR unknown command\r\n"
        };
    }

    private object GetKeyLock(string key)
    {
        lock (this)
        {
            if(!keyLocks.ContainsKey(key))
                keyLocks[key] = new object();

            return keyLocks[key];
        }
    }

    private string HandleBLPOP(string[] arguments)
    {
        var key = arguments[0];
        var keyLock = GetKeyLock(key);

        Console.WriteLine($"Consumer {Thread.CurrentThread.ManagedThreadId} Before Lock");

        lock (keyLock)
        {
            Console.WriteLine($"Consumer {Thread.CurrentThread.ManagedThreadId} After Lock & Before Checking If Key Exist");

            // sleep untill the key is available
            while (!store.ContainsKey(key) || store[key].ListValue?.Count == 0)
            {
                Console.WriteLine($"Consumer {Thread.CurrentThread.ManagedThreadId} Will Sleep Soon");
                Monitor.Wait(keyLock);
            }

            Console.WriteLine($"Consumer {Thread.CurrentThread.ManagedThreadId} After Waking Up");

            var returnedValue = store[key].ListValue[0];
            store[key].ListValue?.RemoveAt(0);

            return $"*2\r\n${key.Length}\r\n{key}\r\n${returnedValue.Length}\r\n{returnedValue}\r\n";
        }
    }

    private string HandleLPOP(string[] arguments)
    {
        string key = arguments[0];
        if (!store.ContainsKey(key))
            return "$-1\r\n";

        var value = store[key];
        if (value.ListValue is null || value.ListValue.Count == 0)
            return "$-1\r\n";

        int numOfElementToRemove = 1;
        if (arguments.Length > 1)
            numOfElementToRemove = Math.Min(int.Parse(arguments[1]) , value.ListValue.Count);

        List<string> removedElements = value.ListValue.Take(numOfElementToRemove).ToList();
        value.ListValue.RemoveRange(0 , numOfElementToRemove);

        var result = new StringBuilder();
        if (arguments.Length > 1)
        {
            result.Append($"*{removedElements.Count}\r\n");
        }

        foreach (var element in removedElements)
            result.Append($"${element.Length}\r\n{element}\r\n");

        return result.ToString();
    }

    private string HandleLEN(string[] arguments)
    {
        string key = arguments[0];

        if (!store.ContainsKey(key))
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

        if (endIdx < 0)
            endIdx = value.ListValue!.Count + endIdx;

        startIdx = Math.Max(0 , startIdx);
        endIdx = Math.Max(0 , endIdx);

        if (endIdx >= value.ListValue!.Count)
            endIdx = value.ListValue.Count - 1;

        if (startIdx >= value.ListValue!.Count || startIdx > endIdx)
            return "*0\r\n";

        var result = new StringBuilder();
        result.Append($"*{endIdx - startIdx + 1}\r\n");

        for (int i = startIdx ; i <= endIdx ; i++)
            result.Append($"${value.ListValue[i].Length}\r\n{value.ListValue[i]}\r\n");

        return result.ToString();
    }

    private string HandleGeneralPush(string[] arguments , Action<RedisValue , List<string>> action)
    {
        var key = arguments[0];
        var values = arguments.Skip(1).ToList();

        var redisValue = new RedisValue();

        Console.WriteLine($"Producer {Thread.CurrentThread.ManagedThreadId} Before Lock");
        var keyLock = GetKeyLock(key);
        lock (keyLock)
        {
            Console.WriteLine($"Producer {Thread.CurrentThread.ManagedThreadId} After Lock");

            if (!store.ContainsKey(key))
                redisValue.ListValue = new();
            else
                redisValue = store[key];

            action(redisValue , values);

            store[key] = redisValue;

            Console.WriteLine($"Producer {Thread.CurrentThread.ManagedThreadId} Before Waking Up All Threads");

            // wake only threads waiting for this key
            Monitor.PulseAll(keyLock);

            Console.WriteLine($"Producer {Thread.CurrentThread.ManagedThreadId} After Waking Up All Threads");


            return $":{store[key]!.ListValue!.Count}\r\n";
        }
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
        return HandleGeneralPush(arguments , (redisValue , values) => redisValue!.ListValue!.AddRange(values));
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
            store.Remove(key , out var _);
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


// When BLPOP is called:
// 1. Check if list has elements
// 2. If yes → pop and return immediately
// 3. If no → add client to waiting queue for that list

// When RPUSH/LPUSH is called:
// 1. Add element to list
// 2. Check if any clients are waiting for this list
// 3. If yes → wake up the longest-waiting client and give them the element

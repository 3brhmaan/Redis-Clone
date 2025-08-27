using codecrafters_redis.src.Core;
using codecrafters_redis.src.Data.Values;
using System.Text;

namespace codecrafters_redis.src.Commands;
public class ZRANGECommand : RedisCommand
{
    public override string Name => "ZRANGE";
    public ZRANGECommand(IServerContext context)
        : base(context) { }

    public override string Execute(string[] arguments)
    {
        var key = arguments[0];
        var startIdx = int.Parse(arguments[1]);
        var lastIdx = int.Parse(arguments[2]);

        if (!storage.ContainsKey(key))
            return "*0\r\n";

        var redisValue = storage.Get(key) as RedisSortedSet;
        var count = redisValue.Set.Count;

        if (startIdx < 0)
        {
            startIdx = Math.Max(0 , count + startIdx);
        }

        if (lastIdx < 0)
        {
            lastIdx = Math.Max(0 , count + lastIdx);
        }

        if (lastIdx >= count)
        {
            lastIdx = count - 1;
        }

        if (startIdx >= count || startIdx > lastIdx)
            return "*0\r\n";

        var result = new StringBuilder();
        result.Append($"*{lastIdx - startIdx + 1}\r\n");

        int idx = 0;
        foreach (var item in redisValue.Set)
        {
            if (idx >= startIdx && idx <= lastIdx)
            {
                result.Append($"${item.value.Length}\r\n{item.value}\r\n");
            }

            idx++;
        }

        return result.ToString();
    }
}

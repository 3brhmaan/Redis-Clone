using codecrafters_redis.src.Core;
using codecrafters_redis.src.Data.Values;

namespace codecrafters_redis.src.Commands;
public class ZCARDCommand : RedisCommand
{
    public override string Name => "ZCARD";
    public ZCARDCommand(IServerContext context)
        : base(context) { }

    public override string Execute(string[] arguments)
    {
        var key = arguments[0];

        if(!storage.ContainsKey(key))
            return ":0\r\n";

        var redisValue = storage.Get(key) as RedisSortedSet;
        var count = redisValue.Set.Count;

        return $":{count}\r\n";
    }
}

using codecrafters_redis.src.Core;
using codecrafters_redis.src.Data.Values;

namespace codecrafters_redis.src.Commands;
public class ZREMCommand : RedisCommand
{
    public override string Name => "ZREM";
    public ZREMCommand(IServerContext context)
        : base(context) { }

    public override string Execute(string[] arguments)
    {
        var key = arguments[0];
        var memberName = arguments[1];

        var redisValue = storage.Get(key) as RedisSortedSet;

        var isMemberExist = redisValue.Set.Any(x => x.value == memberName);
        if (!isMemberExist)
            return ":0\r\n";

        var item = redisValue.Set.First(x => x.value == memberName);
        redisValue.Set.Remove(item);

        return ":1\r\n";
    }
}

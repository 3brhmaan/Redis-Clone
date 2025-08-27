using codecrafters_redis.src.Core;
using codecrafters_redis.src.Data.Values;

namespace codecrafters_redis.src.Commands;
public class ZSCORECommand : RedisCommand
{
    public override string Name => "ZSCORE";
    public ZSCORECommand(IServerContext context)
        : base(context) { }

    public override string Execute(string[] arguments)
    {
        var key = arguments[0];
        var memberName = arguments[1];

        if (!storage.ContainsKey(key))
            return "$-1\r\n";

        var redisValue = storage.Get(key) as RedisSortedSet;

        var isMemberExist = redisValue.Set.Any(x => x.value == memberName);
        if (!isMemberExist)
            return "$-1\r\n";

        var score = redisValue.Set.FirstOrDefault(x => x.value == memberName).key;

        return $"${score.ToString().Length}\r\n{score}\r\n";
    }
}

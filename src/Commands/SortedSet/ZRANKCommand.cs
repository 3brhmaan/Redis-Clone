using codecrafters_redis.src.Commands.Base;
using codecrafters_redis.src.Core;
using codecrafters_redis.src.Storage.Values;

namespace codecrafters_redis.src.Commands.SortedSet;
public class ZRANKCommand : RedisCommand
{
    public override string Name => "ZRANK";
    public ZRANKCommand(IServerContext context)
        : base(context) { }

    public override string Execute(string[] arguments)
    {
        var key = arguments[0];
        var memberName = arguments[1];

        var redisValue = new RedisSortedSet();

        if (storage.ContainsKey(key))
        {
            redisValue = storage.Get(key) as RedisSortedSet;
        }
        else
        {
            return "$-1\r\n";
        }

        int memberRank = -1, index = 0;
        foreach(var item in redisValue.Set)
        {
            if(item.value == memberName)
                memberRank = index;

            index++;
        }

        return memberRank == -1 ? "$-1\r\n" : $":{memberRank}\r\n";
    }
}

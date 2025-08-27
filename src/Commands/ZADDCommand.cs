using codecrafters_redis.src.Core;
using codecrafters_redis.src.Data.Values;

namespace codecrafters_redis.src.Commands;
public class ZADDCommand : RedisCommand
{
    public override string Name => "ZADD";
    public ZADDCommand(IServerContext context)
        : base(context) { }

    public override string Execute(string[] arguments)
    {
        var key = arguments[0];
        var score = double.Parse(arguments[1]);
        var memberName = arguments[2];

        var redisValue = new RedisSortedSet();

        var lockObj = _serverContext.LockManager.GetLock(key);
        lock (lockObj)
        {
            if (storage.ContainsKey(key))
            {
                redisValue = storage.Get(key) as RedisSortedSet;
            }

            var isMemberExist = redisValue.Set.Any(x => x.value == memberName);

            if (isMemberExist)
            {
                var member = redisValue.Set.FirstOrDefault(x => x.value == memberName);
                redisValue.Set.Remove(member);
            }

            redisValue.Set.Add((score , memberName));

            storage.Set(key , redisValue);

            return isMemberExist ? $":0\r\n" : $":1\r\n";
        }
    }
}

using codecrafters_redis.src.Core;
using codecrafters_redis.src.Data.Storage;
using codecrafters_redis.src.Data.Values;
using codecrafters_redis.src.Locking;

namespace codecrafters_redis.src.Commands;
public class LPUSHCommand : RedisCommand
{
    public override string Name => "LPUSH";
    public LPUSHCommand(IServerContext serverContext) 
        : base(serverContext) { }

    public override string Execute(string[] arguments)
    {
        var key = arguments[0];
        var values = arguments.Skip(1).ToList();

        var redisValue = new RedisList();

        var keyLock = lockManager.GetLock(key);
        lock (keyLock)
        {

            if (!storage.ContainsKey(key))
                redisValue.Values = new();
            else
                redisValue = storage.Get(key) as RedisList;

            foreach (var e in values)
                redisValue.Values.Insert(0 , e);

            storage.Set(key , redisValue);

            lockManager.SignalKey(keyLock);

            return $":{(storage.Get(key) as RedisList)!.Values!.Count}\r\n";
        }
    }
}

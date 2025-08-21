using codecrafters_redis.src.Core;
using codecrafters_redis.src.Data.Storage;
using codecrafters_redis.src.Data.Values;
using codecrafters_redis.src.Locking;

namespace codecrafters_redis.src.Commands;
public class LLENCommand : RedisCommand
{
    public override string Name => "LLEN";
    public LLENCommand(IServerContext serverContext) 
        : base(serverContext) { }

    public override string Execute(string[] arguments)
    {
        string key = arguments[0];

        if (!storage.ContainsKey(key))
            return ":0\r\n";

        return $":{(storage.Get(key) as RedisList).Values?.Count}\r\n";
    }
}

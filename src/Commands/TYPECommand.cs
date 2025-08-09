using codecrafters_redis.src.Data.Storage;
using codecrafters_redis.src.Data.Values;
using codecrafters_redis.src.Locking;

namespace codecrafters_redis.src.Commands;
public class TYPECommand : RedisCommand
{
    public override string Name => "TYPE";
    public TYPECommand(IRedisStorage storage , IKeyLockManager lockManager)
        : base(storage , lockManager) { }

    public override string Execute(string[] arguments)
    {
        var key = arguments[0];

        if (!storage.TryGet(key , out var value))
            return "+none\r\n";
        else
            return $"+{value.Type}\r\n";
    }
}

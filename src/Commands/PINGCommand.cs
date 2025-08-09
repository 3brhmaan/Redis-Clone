using codecrafters_redis.src.Data.Storage;
using codecrafters_redis.src.Locking;

namespace codecrafters_redis.src.Commands;
public class PINGCommand : RedisCommand
{
    public override string Name => "PING";
    public PINGCommand(IRedisStorage storage , IKeyLockManager lockManager)
        : base(storage , lockManager) { }

    public override string Execute(string[] arguments)
    {
        return "+PONG\r\n";
    }
}

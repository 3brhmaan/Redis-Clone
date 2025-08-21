using codecrafters_redis.src.Data.Storage;
using codecrafters_redis.src.Locking;

namespace codecrafters_redis.src.Commands;
public class INFOCommand : RedisCommand
{
    public override string Name => "INFO";

    public INFOCommand(IRedisStorage storage , IKeyLockManager lockManager)
        : base(storage , lockManager) { }

    public override string Execute(string[] arguments)
    {
        return $"$11\r\nrole:master\r\n";
    }
}

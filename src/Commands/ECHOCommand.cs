using codecrafters_redis.src.Data.Storage;
using codecrafters_redis.src.Locking;

namespace codecrafters_redis.src.Commands;
internal class ECHOCommand : RedisCommand
{
    public override string Name => "ECHO";
    public ECHOCommand(IRedisStorage storage , IKeyLockManager lockManager)
    : base(storage , lockManager) { }

    public override string Execute(string[] arguments)
    {
        string message = arguments[0];
        return $"+{message}\r\n";
    }
}

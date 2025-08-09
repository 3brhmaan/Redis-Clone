using codecrafters_redis.src.Data.Storage;
using codecrafters_redis.src.Locking;

namespace codecrafters_redis.src.Commands;
public abstract class RedisCommand : IRedisCommand
{
    protected readonly IRedisStorage storage;
    protected readonly IKeyLockManager lockManager;
    public abstract string Name { get; }

    protected RedisCommand(IRedisStorage storage , IKeyLockManager lockManager)
    {
        this.storage = storage;
        this.lockManager = lockManager;
    }

    public abstract string Execute(string[] arguments);
}

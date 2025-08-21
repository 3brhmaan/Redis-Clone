using codecrafters_redis.src.Core;
using codecrafters_redis.src.Data.Storage;
using codecrafters_redis.src.Locking;

namespace codecrafters_redis.src.Commands;
public abstract class RedisCommand : IRedisCommand
{
    protected readonly IServerContext _serverContext;
    protected IRedisStorage storage => _serverContext.Storage;
    protected IKeyLockManager lockManager => _serverContext.LockManager;
    protected RedisServerConfiguration configuration => _serverContext.Configuration;
    public abstract string Name { get; }

    protected RedisCommand(IServerContext serverContext)
    {
        _serverContext = serverContext;
    }

    public abstract string Execute(string[] arguments);
}

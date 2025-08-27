using codecrafters_redis.src.Concurrency.Locking;
using codecrafters_redis.src.Core;
using codecrafters_redis.src.Storage;

namespace codecrafters_redis.src.Commands.Base;
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

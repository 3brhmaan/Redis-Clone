using codecrafters_redis.src.Data.Storage;
using codecrafters_redis.src.Locking;

namespace codecrafters_redis.src.Core;
public class ServerContext : IServerContext
{
    public RedisServerConfiguration Configuration { get; }
    public IRedisStorage Storage { get; }
    public IKeyLockManager LockManager { get; }

    public ServerContext(
        RedisServerConfiguration configuration ,
        IRedisStorage storage ,
        IKeyLockManager lockManager
    )
    {
        Configuration = configuration;
        Storage = storage;
        LockManager = lockManager;
    }
}

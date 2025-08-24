using codecrafters_redis.src.Commands;
using codecrafters_redis.src.Data.Storage;
using codecrafters_redis.src.Locking;

namespace codecrafters_redis.src.Core;
public class ServerContext : IServerContext
{
    public RedisServerConfiguration Configuration { get; }
    public CommandContainer CommandContainer { get; }
    public IRedisStorage Storage { get; }
    public IKeyLockManager LockManager { get; }
    public CommandExecutor CommandExecutor { get; }

    public ServerContext(
        RedisServerConfiguration configuration ,
        IRedisStorage storage ,
        IKeyLockManager lockManager ,
        CommandContainer commandContainer ,
        CommandExecutor commandExecutor)
    {
        Configuration = configuration;
        Storage = storage;
        LockManager = lockManager;
        CommandContainer = commandContainer;
        CommandExecutor = commandExecutor;
    }
}

using codecrafters_redis.src.Commands;
using codecrafters_redis.src.Data.Storage;
using codecrafters_redis.src.Locking;
using codecrafters_redis.src.PubSub;

namespace codecrafters_redis.src.Core;
public interface IServerContext
{
    RedisServerConfiguration Configuration { get; }
    IRedisStorage Storage { get; }
    IKeyLockManager LockManager { get; }
    CommandContainer CommandContainer { get; }
    CommandExecutor CommandExecutor { get; }
    SubscriptionManager SubscriptionManager { get; }
}

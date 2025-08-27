using codecrafters_redis.src.Commands.Container;
using codecrafters_redis.src.Concurrency.Locking;
using codecrafters_redis.src.PubSub;
using codecrafters_redis.src.Storage;

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

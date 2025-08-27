using codecrafters_redis.src.Storage.Values;

namespace codecrafters_redis.src.Storage;
public interface IRedisStorage
{
    bool TryGet(string key , out RedisValue value);
    void Set(string key , RedisValue value);
    bool Remove(string key);
    bool ContainsKey(string key);
    RedisValue Get(string key);
}

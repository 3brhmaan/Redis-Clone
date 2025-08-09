using codecrafters_redis.src.Data.Values;

namespace codecrafters_redis.src.Data.Storage;
public interface IRedisStorage
{
    bool TryGet(string key , out RedisValue value);
    void Set(string key , RedisValue value);
    bool Remove(string key);
    bool ContainsKey(string key);
    RedisValue Get(string key);
}

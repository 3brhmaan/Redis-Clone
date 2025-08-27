using codecrafters_redis.src.Storage.Values;
using System.Collections.Concurrent;

namespace codecrafters_redis.src.Storage;
public class RedisStorage : IRedisStorage
{
    private readonly ConcurrentDictionary<string , RedisValue> store = new();

    public bool ContainsKey(string key)
    {
        return store.ContainsKey(key);
    }

    public bool Remove(string key)
    {
        return store.TryRemove(key , out _);
    }

    public void Set(string key , RedisValue value)
    {
        store[key] = value;
    }

    public RedisValue Get(string key)
    {
        return store[key];
    }

    public bool TryGet(string key , out RedisValue value)
    {
        if (store.TryGetValue(key , out value))
        {
            return true;
        }
        return false;
    }
}

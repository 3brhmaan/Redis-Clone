using System.Collections.Concurrent;

namespace codecrafters_redis.src.Concurrency.Locking;
public class KeyLockManager : IKeyLockManager
{
    private readonly ConcurrentDictionary<string , object> keyLocks = new();

    public object GetLock(string key)
    {
        return keyLocks.GetOrAdd(key , _ => new object());
    }

    public bool WaitForSignal(string key , TimeSpan? timeout = null)
    {
        var lockObj = GetLock(key);
        return timeout.HasValue
            ? Monitor.Wait(lockObj , timeout.Value)
            : Monitor.Wait(lockObj);
    }

    public void SignalKey(object lockObj)
    {
        Monitor.PulseAll(lockObj);
    }
}

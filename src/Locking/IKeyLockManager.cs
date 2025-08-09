namespace codecrafters_redis.src.Locking;
public interface IKeyLockManager
{
    object GetLock(string key);
    bool WaitForSignal(string key , TimeSpan? timeout = null);
    void SignalKey(object lockObj);
}

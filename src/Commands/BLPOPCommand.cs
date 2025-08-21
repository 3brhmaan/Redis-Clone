using codecrafters_redis.src.Core;
using codecrafters_redis.src.Data.Storage;
using codecrafters_redis.src.Data.Values;
using codecrafters_redis.src.Locking;

namespace codecrafters_redis.src.Commands;
public class BLPOPCommand : RedisCommand
{
    public override string Name => "BLPOP";
    public BLPOPCommand(IServerContext serverContext) 
        : base(serverContext) { }

    public override string Execute(string[] arguments)
    {
        var key = arguments[0];
        double timeoutSeconds = double.Parse(arguments[1]);

        var keyLock = lockManager.GetLock(key);

        lock (keyLock)
        {
            // sleep untill the key is available or timeout has elapsed
            while (!storage.ContainsKey(key) || (storage.Get(key) as RedisList)?.Values.Count == 0)
            {
                if (!lockManager.WaitForSignal(key , timeoutSeconds > 0 ? TimeSpan.FromSeconds(timeoutSeconds) : null))
                    return "$-1\r\n";
            }


            var returnedValue = (storage.Get(key) as RedisList).Values[0];
            (storage.Get(key) as RedisList).Values.RemoveAt(0);

            return $"*2\r\n${key.Length}\r\n{key}\r\n${returnedValue.Length}\r\n{returnedValue}\r\n";
        }
    }
}

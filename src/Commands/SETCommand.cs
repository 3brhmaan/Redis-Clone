using codecrafters_redis.src.Core;
using codecrafters_redis.src.Data.Storage;
using codecrafters_redis.src.Data.Values;
using codecrafters_redis.src.Locking;

namespace codecrafters_redis.src.Commands;
public class SETCommand : RedisCommand
{
    public override string Name => "SET";
    public SETCommand(IServerContext serverContext) 
        : base(serverContext) { }

    public override string Execute(string[] arguments)
    {
        string key = arguments[0];
        string value = arguments[1];

        var redisValue = new RedisString(value);

        // Handle expiration (PX option)
        if (arguments.Length > 2 && arguments[2].ToLower() == "px")
        {
            if (int.TryParse(arguments[3] , out int milliseconds))
            {
                redisValue.Expiry = DateTime.UtcNow.AddMilliseconds(milliseconds);
            }
        }

        storage.Set(key , redisValue);
        return "+OK\r\n";
    }
}

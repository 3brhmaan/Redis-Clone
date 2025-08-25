using codecrafters_redis.src.Core;
using codecrafters_redis.src.Data.Storage;
using codecrafters_redis.src.Data.Values;
using codecrafters_redis.src.Locking;

namespace codecrafters_redis.src.Commands;
public class GETCommand : RedisCommand
{
    public override string Name => "GET";

    public GETCommand(IServerContext serverContext)
        : base(serverContext) { }

    public override string Execute(string[] arguments)
    {
        string key = arguments[0];
        var dir = _serverContext.Configuration.RDBdire;
        var filename = _serverContext.Configuration.RDBdbfilename;

        if (!string.IsNullOrEmpty(dir) && !string.IsNullOrEmpty(filename))
        {
            var value = RdbFileHandler.LoadKeysAndValues(dir , filename)[key];

            return $"${value.Length}\r\n{value}\r\n";
        }
        else
        {
            if (!storage.ContainsKey(key))
            {
                return "$-1\r\n";
            }

            var redisValue = storage.Get(key) as RedisString;

            if (redisValue.IsExpired)
            {
                storage.Remove(key);
                return "$-1\r\n";
            }

            return $"${redisValue.Value.Length}\r\n{redisValue.Value}\r\n";
        }
    }
}

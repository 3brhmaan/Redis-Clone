using codecrafters_redis.src.Core;
using codecrafters_redis.src.Data.Storage;
using codecrafters_redis.src.Data.Values;
using codecrafters_redis.src.Locking;

namespace codecrafters_redis.src.Commands;
public class INCRCommand : RedisCommand
{
    public override string Name => "INCR";
    public INCRCommand(IServerContext serverContext) 
        : base(serverContext) { }

    public override string Execute(string[] arguments)
    {
        var key = arguments[0];
        var value = new RedisString("1");

        if (storage.ContainsKey(key))
        {
            value = storage.Get(key) as RedisString;

            if(int.TryParse(value.Value, out var result))
            {
                value.Value = (result + 1).ToString();
            }
            else
            {
                return "-ERR value is not an integer or out of range\r\n";
            }
        }

        storage.Set(key , value);

        return $":{value.Value}\r\n";
    }
}

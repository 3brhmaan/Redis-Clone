using codecrafters_redis.src.Data.Storage;
using codecrafters_redis.src.Data.Values;
using codecrafters_redis.src.Locking;

namespace codecrafters_redis.src.Commands;
public class INCRCommand : RedisCommand
{
    public override string Name => "INCR";
    public INCRCommand(IRedisStorage storage , IKeyLockManager lockManager)
        : base(storage , lockManager) { }

    public override string Execute(string[] arguments)
    {
        var key = arguments[0];
        var value = new RedisString("1");

        if (storage.ContainsKey(key))
        {
            value = storage.Get(key) as RedisString;
            value.Value = (int.Parse(value.Value) + 1).ToString();
        }

        storage.Set(key , value);

        return $":{value.Value}\r\n";
    }
}

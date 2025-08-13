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
        if(storage.TryGet(key, out var value))
        {
            var newValue = value as RedisString;
            newValue.Value = (int.Parse(newValue.Value) + 1).ToString();

            storage.Set(key, newValue);
        }
        else
        {
            var newValue = value as RedisString;
            newValue.Value = "1";

            storage.Set(key, newValue);
        }

        Console.WriteLine((value as RedisString).Value);
        return $":{(value as RedisString).Value}\r\n";
    }
}

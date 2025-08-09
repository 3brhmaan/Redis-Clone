using codecrafters_redis.src.Data.Storage;
using codecrafters_redis.src.Data.Values;
using codecrafters_redis.src.Locking;
using System.Text;

namespace codecrafters_redis.src.Commands;
public class LPOPCommand : RedisCommand
{
    public override string Name => "LPOP";
    public LPOPCommand(IRedisStorage storage , IKeyLockManager lockManager)
        : base(storage , lockManager) { }

    public override string Execute(string[] arguments)
    {
        string key = arguments[0];
        if (!storage.ContainsKey(key))
            return "$-1\r\n";

        var value = storage.Get(key) as RedisList;
        if (value.Values is null || value.Values.Count == 0)
            return "$-1\r\n";

        int numOfElementToRemove = 1;
        if (arguments.Length > 1)
            numOfElementToRemove = Math.Min(int.Parse(arguments[1]) , value.Values.Count);

        List<string> removedElements = value.Values.Take(numOfElementToRemove).ToList();
        value.Values.RemoveRange(0 , numOfElementToRemove);

        var result = new StringBuilder();
        if (arguments.Length > 1)
        {
            result.Append($"*{removedElements.Count}\r\n");
        }

        foreach (var element in removedElements)
            result.Append($"${element.Length}\r\n{element}\r\n");

        return result.ToString();
    }
}

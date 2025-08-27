using codecrafters_redis.src.Commands.Base;
using codecrafters_redis.src.Core;
using codecrafters_redis.src.Storage.Values;
using System.Text;

namespace codecrafters_redis.src.Commands.List;
public class LPOPCommand : RedisCommand
{
    public override string Name => "LPOP";
    public LPOPCommand(IServerContext serverContext) 
        : base(serverContext) { }

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

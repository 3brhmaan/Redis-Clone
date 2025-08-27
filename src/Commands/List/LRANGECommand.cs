using codecrafters_redis.src.Commands.Base;
using codecrafters_redis.src.Core;
using codecrafters_redis.src.Storage.Values;
using System.Text;

namespace codecrafters_redis.src.Commands.List;
public class LRANGECommand : RedisCommand
{
    public override string Name => "LRANGE";
    public LRANGECommand(IServerContext serverContext) 
        : base(serverContext) { }

    public override string Execute(string[] arguments)
    {
        string key = arguments[0];
        int startIdx = int.Parse(arguments[1]);
        int endIdx = int.Parse(arguments[2]);

        if (!storage.ContainsKey(key))
            return "*0\r\n";

        var value = storage.Get(key) as RedisList;

        if (startIdx < 0)
            startIdx = value.Values!.Count + startIdx;

        if (endIdx < 0)
            endIdx = value.Values!.Count + endIdx;

        startIdx = Math.Max(0 , startIdx);
        endIdx = Math.Max(0 , endIdx);

        if (endIdx >= value.Values!.Count)
            endIdx = value.Values.Count - 1;

        if (startIdx >= value.Values!.Count || startIdx > endIdx)
            return "*0\r\n";

        var result = new StringBuilder();
        result.Append($"*{endIdx - startIdx + 1}\r\n");

        for (int i = startIdx ; i <= endIdx ; i++)
            result.Append($"${value.Values[i].Length}\r\n{value.Values[i]}\r\n");

        return result.ToString();
    }
}

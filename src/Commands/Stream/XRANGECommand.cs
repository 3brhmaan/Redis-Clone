using codecrafters_redis.src.Commands.Base;
using codecrafters_redis.src.Core;
using codecrafters_redis.src.Storage.Values;
using System.Text;

namespace codecrafters_redis.src.Commands.Stream;
public class XRANGECommand : RedisCommand
{
    public override string Name => "XRANGE";
    public XRANGECommand(IServerContext serverContext) 
        : base(serverContext) { }

    public override string Execute(string[] arguments)
    {
        var key = arguments[0];
        storage.TryGet(key , out var value);

        var start = arguments[1];
        var end = arguments[2];

        if(start == "-")
        {
            start = (value as RedisStream).Entries[0].Id;
        }

        if(end == "+")
        {
            end = (value as RedisStream).Entries[^1].Id;
        }

        if (!start.Contains("-"))
        {
            start += "-0";
        }

        if (!end.Contains("-"))
        {
            var entry = (value as RedisStream).Entries?.FirstOrDefault(
                (val) => val.Id.Split("-")[0] == start.Split("-")[0]
            );

            if (entry is not null)
            {
                var entrySequenceNumber = entry.Id.Split("-")[1];
                end += $"-{entrySequenceNumber}";
            }
        }

        var entries = (value as RedisStream)?.Entries?.Where(
            (entry) => string.Compare(entry.Id , start) >= 0 && string.Compare(entry.Id , end) <= 0
        ).ToList();

        var result = new StringBuilder();
        result.Append($"*{entries?.Count}\r\n");

        foreach (var entry in entries)
        {
            result.Append("*2\r\n");
            result.Append($"${entry.Id.Length}\r\n{entry.Id}\r\n");
            result.Append($"*{entry.Fields.Count * 2}\r\n");

            foreach (var pair in entry.Fields)
            {
                result.Append($"${pair.Key.Length}\r\n{pair.Key}\r\n");
                result.Append($"${pair.Value.Length}\r\n{pair.Value}\r\n");
            }
        }

        return result.ToString();
    }
}

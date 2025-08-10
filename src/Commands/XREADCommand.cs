using codecrafters_redis.src.Data.Storage;
using codecrafters_redis.src.Data.Values;
using codecrafters_redis.src.Locking;
using System.Text;

namespace codecrafters_redis.src.Commands;
public class XREADCommand : RedisCommand
{
    public override string Name => "XREAD";
    public XREADCommand(IRedisStorage storage , IKeyLockManager lockManager)
        : base(storage , lockManager) { }

    public override string Execute(string[] arguments)
    {
        var key = arguments[1];
        var id = arguments[2];

        storage.TryGet(key, out var value);

        var entries = (value as RedisStream)?.Entries?.Where(
            (entry) => string.Compare(entry.Id , id) > 0
        ).ToList();

        var result = new StringBuilder();
        result.Append($"*1\r\n*2\r\n${key.Length}\r\n{key}\r\n");
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

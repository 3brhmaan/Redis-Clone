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
        var keys = arguments.Skip(1).Where(x => !x.Contains("-")).ToList();
        var ids = arguments.Skip(1).Where(x => x.Contains("-")).ToList();

        int wait = -1;

        if (arguments[0] == "block")
        {
            wait = int.Parse(arguments[1]);
            keys = arguments.Skip(3).Where(x => !x.Contains("-")).ToList();
            ids = arguments.Where(x => x.Contains("-")).ToList();
        }

        if(wait != -1)
        {
            Thread.Sleep(wait);
        }

        var values = new List<RedisValue>();
        foreach (var key in keys)
        {
            values.Add(storage.Get(key));
        }

        var valuesEntries = new List<List<RedisStreamEntry>>();
        for(int i=0; i<ids.Count; i++)
        {
            var valueEntries = (values[i] as RedisStream)?.Entries?
                .Where(e => string.Compare(e.Id, ids[i]) > 0).ToList();

            if(valueEntries != null && valueEntries.Count > 0)
            {
                valuesEntries.Add(valueEntries);
            }
        }

        if(valuesEntries.Count == 0)
        {
            return "$-1\r\n";
        }

        var result = new StringBuilder();
        result.Append($"*{valuesEntries.Count}\r\n");
        
        for(int i=0; i<valuesEntries.Count; i++)
        {
            result.Append($"*2\r\n");
            result.Append($"${keys[i].Length}\r\n{keys[i]}\r\n");
            result.Append($"*{valuesEntries[i]?.Count}\r\n");

            foreach (var entry in valuesEntries[i])
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
        }

        return result.ToString();
    }
}

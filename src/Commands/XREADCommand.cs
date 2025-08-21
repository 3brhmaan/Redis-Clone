using codecrafters_redis.src.Core;
using codecrafters_redis.src.Data.Storage;
using codecrafters_redis.src.Data.Values;
using codecrafters_redis.src.Locking;
using System.Text;

namespace codecrafters_redis.src.Commands;
public class XREADCommand : RedisCommand
{
    public override string Name => "XREAD";
    public XREADCommand(IServerContext serverContext) 
        : base(serverContext) { }

    public override string Execute(string[] arguments)
    {

        if (arguments[0] == "block")
        {
            int wait = int.Parse(arguments[1]);
            return HandleBlockingReads(wait , arguments.Skip(3).ToList());
        }
        else
        {
            return HandleNormalReads(arguments.Skip(1).ToList());
        }
    }

    private string HandleBlockingReads(int wait , List<string> arguments)
    {
        var id = arguments[^1];
        var key = arguments[^2];

        var value = storage.Get(key) as RedisStream;

        if (wait != 0)
        {
            Thread.Sleep(wait);

            id = (id == "$") ? value.Entries[^1].Id : id;
        }
        else
        {
            var lockObj = lockManager.GetLock(key);

            lock (lockObj)
            {
                while (value.Entries.Count == 1)
                {
                    lockManager.WaitForSignal(key);
                }

                id = (id == "$") ? value.Entries[^2].Id : id;
            }
        }


        return BuildStreamEntries(new List<string> { key } , new List<string> { id });
    }

    private string HandleNormalReads(List<string> arguments)
    {
        var keys = arguments.Where(x => !x.Contains("-")).ToList();
        var ids = arguments.TakeLast(keys.Count).ToList();

        return BuildStreamEntries(keys , ids);
    }

    private string BuildStreamEntries(List<string> keys , List<string> ids)
    {
        var values = new List<RedisValue>();
        foreach (var key in keys)
        {
            values.Add(storage.Get(key));
        }

        var valuesEntries = new List<List<RedisStreamEntry>>();
        for (int i = 0 ; i < ids.Count ; i++)
        {
            var valueEntries = (values[i] as RedisStream)?.Entries?
                .Where(e => string.Compare(e.Id , ids[i]) > 0).ToList();

            if (valueEntries != null && valueEntries.Count > 0)
            {
                valuesEntries.Add(valueEntries);
            }
        }

        if (valuesEntries.Count == 0)
        {
            return "$-1\r\n";
        }

        var result = new StringBuilder();
        result.Append($"*{valuesEntries.Count}\r\n");

        for (int i = 0 ; i < valuesEntries.Count ; i++)
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


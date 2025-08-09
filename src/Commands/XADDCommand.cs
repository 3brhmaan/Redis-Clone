using codecrafters_redis.src.Data.Storage;
using codecrafters_redis.src.Data.Values;
using codecrafters_redis.src.Locking;

namespace codecrafters_redis.src.Commands;
internal class XADDCommand : RedisCommand
{
    public override string Name => "XADD";
    public XADDCommand(IRedisStorage storage , IKeyLockManager lockManager)
        : base(storage , lockManager) { }

    public override string Execute(string[] arguments)
    {
        var key = arguments[0];
        var id = arguments[1];

        if (!ValidateStremEntryId(id , key , out var resultMessage))
        {
            return $"-ERR {resultMessage}\r\n";
        }

        RedisStream value = new();
        if (storage.ContainsKey(key))
            value = storage.Get(key) as RedisStream;

        id = GenerateStreamEntryId(id , value);

        var streamEntry = new RedisStreamEntry { Id = id };

        for (int i = 2 ; i < arguments.Length ; i += 2)
        {
            streamEntry.Fields.Add(arguments[i] , arguments[i + 1]);
        }

        value.Entries?.Add(streamEntry);

        storage.Set(key , value);

        return $"${id.Length}\r\n{id}\r\n";
    }
    private bool ValidateStremEntryId(string id , string key , out string resultMessage)
    {
        if (id[^1] == '*')
        {
            resultMessage = "";
            return true;
        }
        else if (id == "0-0")
        {
            resultMessage = "The ID specified in XADD must be greater than 0-0";
            return false;
        }
        else
        {
            var idParts = id.Split('-');

            if (storage.TryGet(key , out var value))
            {
                var lastIdParts = (value as RedisStream)?.Entries[^1].Id.Split('-');

                if (string.Compare(idParts[0] , lastIdParts[0]) > 0)
                {
                    resultMessage = "";
                    return true;
                }
                else if (idParts[0] == lastIdParts[0] && string.Compare(idParts[1] , lastIdParts[1]) > 0)
                {
                    resultMessage = "";
                    return true;
                }
                else
                {
                    resultMessage = "The ID specified in XADD is equal or smaller than the target stream top item";
                    return false;
                }
            }
            else
            {
                resultMessage = "";
                return true;
            }
        }
    }
    private string GenerateStreamEntryId(string id , RedisStream value)
    {
        if (id == "*")
        {
            // full auto-generate id

            string timeInMS = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
            var entry = value.Entries?.FirstOrDefault(
                (val) => val.Id.Split("-")[0] == timeInMS
            );

            if (entry is not null)
                return $"{timeInMS}-{(int.Parse(entry.Id.Split("-")[1]) + 1).ToString()}";
            else
                return $"{timeInMS}-0";
        }
        else if (id[^1] == '*')
        {
            // partial auto-generate id

            var idParts = id.Split("-");

            if (value.Entries?.Count == 0)
            {
                idParts[1] = idParts[0] != "0" ? "0" : "1";
            }
            else
            {
                var lastIdParts = value.Entries[^1].Id.Split("-");
                if (idParts[0] == lastIdParts[0])
                {
                    idParts[1] = (int.Parse(lastIdParts[1]) + 1).ToString();
                }
                else
                {
                    idParts[1] = "0";
                }
            }

            return string.Join("-" , idParts);
        }
        else
        {
            // explicit id

            return id;
        }
    }
}

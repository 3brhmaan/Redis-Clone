using codecrafters_redis.src.Commands.Base;
using codecrafters_redis.src.Core;
using codecrafters_redis.src.Storage;
using System.Text;

namespace codecrafters_redis.src.Commands.Server;
internal class KEYSCommand : RedisCommand
{
    public override string Name => "KEYS";
    public KEYSCommand(IServerContext context)
        : base(context) { }

    public override string Execute(string[] arguments)
    {
        var pattern = arguments[0];
        var dir = _serverContext.Configuration.RDBdire;
        var filename = _serverContext.Configuration.RDBdbfilename;

        var keys = RdbFileHandler.LoadKeysAndValues(dir , filename).Keys.ToList();

        var result = new StringBuilder();
        result.Append($"*{keys.Count}\r\n");
        foreach (var key in keys)
        {
            result.Append($"${key.Length}\r\n{key}\r\n");
        }

        return result.ToString();
    }
}

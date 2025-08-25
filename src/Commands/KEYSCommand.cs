using codecrafters_redis.src.Core;
using System.Text;

namespace codecrafters_redis.src.Commands;
internal class KEYSCommand : RedisCommand
{
    public override string Name => "KEYS";
    public KEYSCommand(IServerContext context)
        : base(context) { }

    public override string Execute(string[] arguments)
    {
        var pattern = arguments[0];

        var keys = RdbFileHandler.LoadKeys(_serverContext.Configuration.RDBdire, _serverContext.Configuration.RDBdbfilename);

        return $"*1\r\n${keys[0].Length}\r\n{keys[0]}\r\n";
    }
}

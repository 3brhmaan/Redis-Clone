using codecrafters_redis.src.Commands.Base;
using codecrafters_redis.src.Core;

namespace codecrafters_redis.src.Commands.Server;
public class TYPECommand : RedisCommand
{
    public override string Name => "TYPE";
    public TYPECommand(IServerContext serverContext) 
        : base(serverContext) { }

    public override string Execute(string[] arguments)
    {
        var key = arguments[0];

        if (!storage.TryGet(key , out var value))
            return "+none\r\n";
        else
            return $"+{value.Type}\r\n";
    }
}

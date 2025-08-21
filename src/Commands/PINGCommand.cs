using codecrafters_redis.src.Core;
using codecrafters_redis.src.Data.Storage;
using codecrafters_redis.src.Locking;

namespace codecrafters_redis.src.Commands;
public class PINGCommand : RedisCommand
{
    public override string Name => "PING";
    public PINGCommand(IServerContext serverContext) 
        : base(serverContext) { }

    public override string Execute(string[] arguments)
    {
        return "+PONG\r\n";
    }
}

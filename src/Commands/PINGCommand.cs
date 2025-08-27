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
        if (_serverContext.SubscriptionManager.IsInSubscribeMode)
        {
            return "*2\r\n$4\r\npong\r\n$0\r\n\r\n";
        }
        else
        {
            return "+PONG\r\n";

        }
    }
}

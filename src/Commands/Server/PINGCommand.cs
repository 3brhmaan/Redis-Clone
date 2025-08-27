using codecrafters_redis.src.Commands.Base;
using codecrafters_redis.src.Core;

namespace codecrafters_redis.src.Commands.Server;
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

using codecrafters_redis.src.Core;

namespace codecrafters_redis.src.Commands;
public class PUBLISHCommand : RedisCommand
{
    public override string Name => "PUBLISH";
    public PUBLISHCommand(IServerContext context) 
        : base(context) { }

    public override string Execute(string[] arguments)
    {
        var channel = arguments[0];
        var message = arguments[1];

        var channelSockets = _serverContext.SubscriptionManager.GetChannelSockts(channel);

        return $":{channelSockets.Count}\r\n";
    }
}

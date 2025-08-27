using codecrafters_redis.src.Commands.Base;
using codecrafters_redis.src.Core;

namespace codecrafters_redis.src.Commands.PubSub;
public class UNSUBSCRIBECommand : RedisCommand
{
    public override string Name => "UNSUBSCRIBE";
    public UNSUBSCRIBECommand(IServerContext context)
        : base(context) { }

    public override string Execute(string[] arguments)
    {
        var channel = arguments[0];

        var channelsCount = _serverContext.SubscriptionManager.Unsubscribe(channel);

        return $"*3\r\n$11\r\nunsubscribe\r\n${channel.Length}\r\n{channel}\r\n:{channelsCount}\r\n";
    }
}

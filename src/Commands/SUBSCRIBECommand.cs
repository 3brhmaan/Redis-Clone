using codecrafters_redis.src.Core;

namespace codecrafters_redis.src.Commands;
public class SUBSCRIBECommand : RedisCommand
{
    public override string Name => "SUBSCRIBE";
    public SUBSCRIBECommand(IServerContext context) 
        : base(context) { }

    public override string Execute(string[] arguments)
    {
        var channelName = arguments[0];

        var channelsCount = _serverContext.SubscriptionManager.Subscribe(channelName);

        return $"*3\r\n$9\r\nsubscribe\r\n${channelName.Length}\r\n{channelName}\r\n:{channelsCount}\r\n";
    }
}
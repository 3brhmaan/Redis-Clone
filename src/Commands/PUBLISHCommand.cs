using codecrafters_redis.src.Core;
using System.Text;

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

        var response = $"*3\r\n$7\r\nmessage\r\n${channel.Length}\r\n{channel}\r\n${message.Length}\r\n{message}\r\n";

        foreach(var socket in channelSockets)
        {
            socket.Send(Encoding.UTF8.GetBytes(response));
        }

        return $":{channelSockets.Count}\r\n";
    }
}

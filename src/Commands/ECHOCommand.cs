using codecrafters_redis.src.Core;

namespace codecrafters_redis.src.Commands;
public class ECHOCommand : RedisCommand
{
    public override string Name => "ECHO";
    public ECHOCommand(IServerContext serverContext) 
        : base(serverContext) { }

    public override string Execute(string[] arguments)
    {
        string message = arguments[0];
        return $"+{message}\r\n";
    }
}

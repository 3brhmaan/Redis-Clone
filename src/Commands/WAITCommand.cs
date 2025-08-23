using codecrafters_redis.src.Core;

namespace codecrafters_redis.src.Commands;
internal class WAITCommand : RedisCommand
{
    public override string Name => "WAIT";
    public WAITCommand(IServerContext serverContext): base(serverContext) { }

    public override string Execute(string[] arguments)
    {
        return $":{configuration.ReplicaCount}\r\n";
    }
}

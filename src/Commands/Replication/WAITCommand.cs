using codecrafters_redis.src.Commands.Base;
using codecrafters_redis.src.Core;

namespace codecrafters_redis.src.Commands.Replication;
internal class WAITCommand : RedisCommand
{
    public override string Name => "WAIT";
    public WAITCommand(IServerContext serverContext) : base(serverContext) { }

    public override string Execute(string[] arguments)
    {
        // TODO: Last Replication Stage
        int minNumberOfReplicas = int.Parse(arguments[0]);
        int timeout = int.Parse(arguments[1]);

        Console.WriteLine($"Waitning {minNumberOfReplicas} for {timeout}");

        return $":{configuration.ReplicaConnection.Count}\r\n";
    }
}

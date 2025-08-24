using codecrafters_redis.src.Core;

namespace codecrafters_redis.src.Commands;
internal class WAITCommand : RedisCommand
{
    public override string Name => "WAIT";
    public WAITCommand(IServerContext serverContext) : base(serverContext) { }

    public override string Execute(string[] arguments)
    {
        int minNumberOfReplicas = int.Parse(arguments[0]);
        int timeout = int.Parse(arguments[1]);

        Console.WriteLine($"Waitning {minNumberOfReplicas} for {timeout}");

        //var request = "REPLCONF\r\nGETACK\r\n*\r\n";
        //var response = _serverContext.CommandExecutor.Execute(request, "ff");
        //Console.WriteLine($"REPLCONF Response from WAIT: {response}");


        return $":{configuration.ReplicaConnection.Count}\r\n";
    }
}

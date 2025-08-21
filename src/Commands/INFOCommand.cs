using codecrafters_redis.src.Core;
using codecrafters_redis.src.Data.Storage;
using codecrafters_redis.src.Locking;

namespace codecrafters_redis.src.Commands;
public class INFOCommand : RedisCommand
{
    public override string Name => "INFO";

    public INFOCommand(IServerContext serverContext)
        : base(serverContext) { }

    public override string Execute(string[] arguments)
    {
        string result = "";
        switch (configuration.ReplicationMode)
        {
            case ReplicationMode.Master:
                result = $"$11\r\nrole:master\r\n";
                break;
            case ReplicationMode.Slave:
                result = $"$10\r\nrole:slave\r\n";
                break;
        }

        return result;
    }
}

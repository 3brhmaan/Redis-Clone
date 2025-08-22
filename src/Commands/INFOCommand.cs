using codecrafters_redis.src.Core;
using System.Text;

namespace codecrafters_redis.src.Commands;
public class INFOCommand : RedisCommand
{
    public override string Name => "INFO";

    public INFOCommand(IServerContext serverContext)
        : base(serverContext) { }

    public override string Execute(string[] arguments)
    {
        StringBuilder result = new ();
        switch (configuration.ReplicationMode)
        {
            case ReplicationMode.Master:
                int len = "role:master".Length + 
                    "master_replid".Length + 41 + 
                    "master_repl_offset".Length + configuration.MasterReplOffset.ToString().Length + 1;

                result.Append($"${len}\r\nrole:master");
                result.Append(
                    $"master_replid:{configuration.MasterReplId}"
                );
                result.Append(
                    $"master_repl_offset:{configuration.MasterReplOffset}\r\n"
                );
                break;
            case ReplicationMode.Slave:
                result.Append($"$10\r\nrole:slave\r\n");
                break;
        }

        return result.ToString();
    }
}

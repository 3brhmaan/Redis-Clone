using codecrafters_redis.src.Core;
using System.Net.Sockets;
using System.Text;

namespace codecrafters_redis.src.Commands;
public class PSYNCCommand : RedisCommand
{
    public PSYNCCommand(IServerContext serverContext) 
        : base(serverContext) { }

    public override string Name => "PSYNC";

    public override string Execute(string[] arguments)
    {
        return $"+FULLRESYNC {configuration.MasterReplId} 0\r\n";
    }
}

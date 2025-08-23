using codecrafters_redis.src.Core;

namespace codecrafters_redis.src.Commands;
public class REPLCONFCommand : RedisCommand
{
    public override string Name => "REPLCONF";

    public REPLCONFCommand(IServerContext context)
        : base(context) { }

    public override string Execute(string[] arguments)
    {
        // for future we can montior the slave port and capalities using the arguments

        if (arguments.Contains("GETACK" , StringComparer.OrdinalIgnoreCase))
        {
            return "*3\r\n$8\r\nREPLCONF\r\n$3\r\nACK\r\n$1\r\n0\r\n";
        }

        return "+OK\r\n";
    }
}

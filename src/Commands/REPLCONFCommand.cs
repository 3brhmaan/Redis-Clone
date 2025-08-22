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

        return "+OK\r\n";
    }
}

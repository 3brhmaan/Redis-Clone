using codecrafters_redis.src.Commands.Base;
using codecrafters_redis.src.Core;

namespace codecrafters_redis.src.Commands.Server;
public class CONFIGCommand : RedisCommand
{
    public override string Name => "CONFIG";
    public CONFIGCommand(IServerContext serverContext) 
        : base(serverContext){}

    public override string Execute(string[] arguments)
    {
        switch (arguments[1])
        {
            case "dir":
                var dir = _serverContext.Configuration.RDBdire;

                return $"*2\r\n$3\r\ndir\r\n${dir.Length}\r\n{dir}\r\n";
            case "dbfilename":
                var dbfilename = _serverContext.Configuration.RDBdbfilename;

                return $"*2\r\n$10\r\ndbfilename\r\n${dbfilename.Length}\r\n{dbfilename}\r\n";
            default:
                return "";
        }
    }
}

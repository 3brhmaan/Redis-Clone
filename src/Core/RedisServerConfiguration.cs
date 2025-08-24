using System.Net.Sockets;

namespace codecrafters_redis.src.Core;
public class RedisServerConfiguration
{
    // Server settings
    public int Port { get; set; } = 6379;
    public ReplicationMode ReplicationMode { get; set; } = ReplicationMode.Master;
    public string RDBdire { get; set; }
    public string RDBdbfilename { get; set; }

    // Remote master settings (when this server is a slave)
    public string? MasterHost { get; set; }
    public int? MasterPort { get; set; }

    // Master settings (when this server is a master)
    public string MasterReplId { get; } = "8371b4fb1155b71f4a04d3e1bc3e18c4a990aeeb";
    public int MasterReplOffset { get; set; } = 0; // replica/master offset
    public List<Socket> ReplicaConnection { get; set; } = new();

    public static RedisServerConfiguration ParseArguments(string[] args)
    {
        var config = new RedisServerConfiguration();

        for (int i = 0 ; i < args.Length ; i++)
        {
            switch (args[i])
            {
                case "--port":
                    config.Port = int.Parse(args[++i]);
                    break;
                case "--replicaof":
                    var masterInfo = args[++i].Split(' ');
                    config.ReplicationMode = ReplicationMode.Slave;
                    config.MasterHost = masterInfo[0];
                    config.MasterPort = int.Parse(masterInfo[1]);
                    break;
                case "--dir":
                    config.RDBdire = args[++i];
                    break;
                case "--dbfilename":
                    config.RDBdbfilename = args[++i];
                    break;
            }
        }

        return config;
    }
}

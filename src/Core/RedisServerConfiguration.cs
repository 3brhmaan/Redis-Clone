namespace codecrafters_redis.src.Core;
public class RedisServerConfiguration
{
    public int Port { get; set; } = 6379;
    public ReplicationMode ReplicationMode { get; set; } = ReplicationMode.Master;
    public string? MasterHost { get; set; }
    public int? MasterPort { get; set; }
    public string ServerId { get; set; } = Guid.NewGuid().ToString();

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
            }
        }

        return config;
    }
}

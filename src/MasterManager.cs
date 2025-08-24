using codecrafters_redis.src.Core;
using System.Net.Sockets;
using System.Text;

namespace codecrafters_redis.src;
public class MasterManager
{
    private readonly RedisServerConfiguration configuration;

    public MasterManager(RedisServerConfiguration configuration)
    {
        this.configuration = configuration;
    }

    public void PropagateWriteCommandsToReplicas(string request)
    {
        if (IsWriteCommand(request))
        {
            Console.WriteLine($"Propagating {request.Length} to Replica, {request}");

            foreach (var replica in configuration.ReplicaConnection)
            {
                replica.Send(Encoding.UTF8.GetBytes(request));
            }
        }
    }
    public void SendEmptyRDBFile(Socket client)
    {
        string base64RDB =
            "UkVESVMwMDEx+glyZWRpcy12ZXIFNy4yLjD6CnJlZGlzLWJpdHPAQPoFY3RpbWXCbQi8ZfoIdXNlZC1tZW3CsMQQAPoIYW9mLWJhc2XAAP/wbjv+wP9aog==";

        byte[] rdbBytes = Convert.FromBase64String(base64RDB);

        string header = $"${rdbBytes.Length}\r\n";

        client.Send(Encoding.UTF8.GetBytes(header));

        client.Send(rdbBytes);
    }
    private bool IsWriteCommand(string command)
    {
        List<string> writeCommands = ["SET"];

        foreach (var writeCommand in writeCommands)
        {
            if (command.Contains(writeCommand))
                return true;
        }

        return false;
    }
}

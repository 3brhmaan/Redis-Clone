using codecrafters_redis.src.Commands;
using codecrafters_redis.src.Core;
using codecrafters_redis.src.Locking;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace codecrafters_redis.src;
public class RedisServer
{
    private readonly IServerContext context;
    private readonly MasterManager masterManager;
    private readonly ReplicaManager replicaManager;

    public RedisServer(IServerContext context)
    {
        this.context = context;

        replicaManager = new ReplicaManager(context.CommandExecutor, context.Configuration);
        masterManager = new MasterManager(context.Configuration);
    }

    public void Start()
    {
        var server = new TcpListener(IPAddress.Any , context.Configuration.Port);

        server.Start();

        if (context.Configuration.ReplicationMode == ReplicationMode.Slave)
        {
            replicaManager.EstablishHandshake(out var client , out var overflowCommand);
            _ = Task.Run(() => replicaManager.StartListeningToMaster(client , overflowCommand));
        }

        while (true)
        {
            var client = server.AcceptSocket(); // waiting connection

            _ = Task.Run(() => HandleClient(client));
        }
    }
    private void HandleClient(Socket client)
    {
        try
        {
            while (client.Connected)
            {
                byte[] buffer = new byte[1024];
                int bytesRead = client.Receive(buffer);

                // Check if connection was closed
                if (bytesRead == 0)
                {
                    break; // Client disconnected
                }

                string request = Encoding.UTF8.GetString(buffer , 0 , bytesRead);
                string response = context.CommandExecutor.Execute(request , client);
                client.Send(Encoding.UTF8.GetBytes(response));


                // during handshake save the connection and send Empty RDB File
                if (request.Contains("PSYNC"))
                {
                    masterManager.SendEmptyRDBFile(client);
                    context.Configuration.ReplicaConnection.Add(client);

                    continue;
                }

                if (context.Configuration.ReplicationMode == ReplicationMode.Master)
                {
                    masterManager.PropagateWriteCommandsToReplicas(request);
                }
            }

            client.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }
}

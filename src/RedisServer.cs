using codecrafters_redis.src.Core;
using codecrafters_redis.src.Transactions;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace codecrafters_redis.src;
public class RedisServer
{
    private readonly TcpListener server;
    private readonly RedisRequestProcessor commandHandler;
    private readonly RedisServerConfiguration configuration;

    public RedisServer(RedisRequestProcessor commandHandler , RedisServerConfiguration configuration)
    {
        this.commandHandler = commandHandler;
        this.configuration = configuration;

        server = new TcpListener(IPAddress.Any , configuration.Port);
    }

    public void Start()
    {
        if(configuration.ReplicationMode == ReplicationMode.Slave)
        {
            var master = new TcpClient(
                configuration.MasterHost, configuration.MasterPort.Value
            );

            var stream = master.GetStream();

            stream.Write(Encoding.UTF8.GetBytes("*1\r\n$4\r\nPING\r\n"));
        }

        server.Start();

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
            string clientId = client.RemoteEndPoint?.ToString()!;

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
                string response = commandHandler.ParseRedisCommand(request , clientId);

                client.Send(Encoding.UTF8.GetBytes(response));
            }

            client.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }
}

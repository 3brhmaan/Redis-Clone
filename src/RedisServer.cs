using codecrafters_redis.src.Transactions;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace codecrafters_redis.src;
public class RedisServer
{
    private readonly TcpListener server;
    private readonly RedisCommandHandler commandHandler;
    private readonly int port;

    public RedisServer(RedisCommandHandler commandHandler , int port = 6379)
    {
        this.port = port;
        this.commandHandler = commandHandler;

        server = new TcpListener(IPAddress.Any , port);
    }

    public void Start()
    {
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
            TransactionManager.Instance.SetCurrentClientId(clientId);

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
                string response = commandHandler.ParseRedisCommand(request);

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

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
        if (configuration.ReplicationMode == ReplicationMode.Slave)
        {
            EstablishReplicationHandshake();
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

                if (request.Contains("PSYNC"))
                {
                    // send empty RDB after the 3 handshake response have been sended
                    SendEmptyRDBFile(client);
                }
            }

            client.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }
    private void SendEmptyRDBFile(Socket client)
    {
        string base64RDB = "UkVESVMwMDEx+glyZWRpcy12ZXIFNy4yLjD6CnJlZGlzLWJpdHPAQPoFY3RpbWXCbQi8ZfoIdXNlZC1tZW3CsMQQAPoIYW9mLWJhc2XAAP/wbjv+wP9aog==";

        byte[] rdbBytes = Convert.FromBase64String(base64RDB);

        string header = $"${rdbBytes.Length}\r\n";

        client.Send(Encoding.UTF8.GetBytes(header));

        client.Send(rdbBytes);
    }
    private void EstablishReplicationHandshake()
    {
        // 3 handshake between slave and master
        var master = new TcpClient(
            configuration.MasterHost , configuration.MasterPort.Value
        );

        var stream = master.GetStream();

        //PING command
        string pingCommand = "*1\r\n$4\r\nPING\r\n";

        if (SendAndVerify(stream , pingCommand , "PONG"))
        {
            // REPLCONF command (listening-port)
            string replicaPort = configuration.Port.ToString();
            string replconf1 = $"*3\r\n$8\r\nREPLCONF\r\n$14\r\nlistening-port\r\n${replicaPort.Length}\r\n{replicaPort}\r\n";

            if (SendAndVerify(stream , replconf1 , "OK"))
            {
                // REPLCONF command (capabilities)
                string replconf2 = "*3\r\n$8\r\nREPLCONF\r\n$4\r\ncapa\r\n$6\r\npsync2\r\n";

                // PSYNC Command with <master replication id> and <offset>
                string psyncCommand = "*3\r\n$5\r\nPSYNC\r\n$1\r\n?\r\n$2\r\n-1\r\n";
                if (SendAndVerify(stream , replconf2 , "OK"))
                {
                    SendAndVerify(stream , psyncCommand , "FULLRESYNC");
                }
            }
        }
    }
    private bool SendAndVerify(NetworkStream stream , string command , string expected)
    {
        stream.Write(Encoding.UTF8.GetBytes(command));

        byte[] buffer = new byte[1024];
        int bytesRead = stream.Read(buffer , 0 , buffer.Length);
        string response = Encoding.UTF8.GetString(buffer , 0 , bytesRead);

        return response.Contains(expected);
    }
}

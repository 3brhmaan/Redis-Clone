using codecrafters_redis.src.Core;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace codecrafters_redis.src;
public class RedisServer
{
    private readonly TcpListener server;
    private readonly RedisRequestProcessor commandHandler;
    private readonly RedisServerConfiguration configuration;
    private readonly List<Socket> replicasConnection = new();

    public RedisServer(RedisRequestProcessor commandHandler , RedisServerConfiguration configuration)
    {
        this.commandHandler = commandHandler;
        this.configuration = configuration;

        server = new TcpListener(IPAddress.Any , configuration.Port);
    }

    public void Start()
    {
        server.Start();
        //Console.WriteLine($"Redis server started on port {configuration.Port}");

        if (configuration.ReplicationMode == ReplicationMode.Slave)
        {
            //Console.WriteLine("Estaplishing Replication Handshake...");
            EstablishReplicationHandshake(out var client , out var overflowCommand);
            //Console.WriteLine("Handshake completed");
            _ = Task.Run(() => StartListeningToMaster(client , overflowCommand));
        }

        while (true)
        {
            //Console.WriteLine("Wating for connection");
            var client = server.AcceptSocket(); // waiting connection
            //Console.WriteLine("Got a Connection");

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


                // during handshake save the connection and send Empty RDB File
                if (request.Contains("PSYNC"))
                {
                    SendEmptyRDBFile(client);
                    replicasConnection.Add(client);
                    configuration.ReplicaCount ++;

                    continue;
                }

                if (configuration.ReplicationMode == ReplicationMode.Master)
                {
                    PropagateWriteCommandsToReplicas(request);
                }
            }

            client.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }

    private void PropagateWriteCommandsToReplicas(string request)
    {
        if (IsWriteCommand(request))
        {
            foreach (var replica in replicasConnection)
            {
                replica.Send(Encoding.UTF8.GetBytes(request));
            }
        }
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

    private void StartListeningToMaster(Socket client , string overflowCommand)
    {
        string clientId = client.RemoteEndPoint?.ToString()!;

        if (overflowCommand is not null)
        {
            string overflowResponse = commandHandler.ParseRedisCommand(overflowCommand , clientId);
            if (overflowCommand.Contains("REPLCONF"))
            {
                configuration.MasterReplOffset += overflowCommand.Length;
                client.Send(Encoding.UTF8.GetBytes(overflowResponse));
            }
        }

        //Console.WriteLine("Start Listening to Master Request");
        try
        {
            while (client.Connected)
            {
                byte[] buffer = new byte[4096];
                int bytesRead = client.Receive(buffer);

                // Check if connection was closed
                if (bytesRead == 0)
                {
                    //Console.WriteLine("Replica Read No Data From Master");
                    break; // Client disconnected
                }

                string tcpRequest = Encoding.UTF8.GetString(buffer , 0 , bytesRead);

                //Console.WriteLine($"Replica Listen To Master Request: {tcpRequest}");

                List<string> strRequests = tcpRequest.Split("\r\n" , StringSplitOptions.RemoveEmptyEntries).ToList();
                List<string> requests = new();
                StringBuilder str = new();

                for (int i = 0 ; i < strRequests.Count ; i++)
                {
                    if (strRequests[i].StartsWith("*") && strRequests[i].Trim().Length > 1)
                    {
                        if (str.Length > 0)
                        {
                            requests.Add(str.ToString());
                            str.Clear();
                        }
                    }

                    str.Append(strRequests[i] + "\r\n");
                }

                if (str.Length > 0)
                {
                    requests.Add(str.ToString());
                }


                foreach (string request in requests)
                {
                    //Console.WriteLine($"Readig {request.Length} bytes from {request}");
                    string response = commandHandler.ParseRedisCommand(request , clientId);
                    configuration.MasterReplOffset += request.Length;
                    if (request.Contains("REPLCONF"))
                    {
                        client.Send(Encoding.UTF8.GetBytes(response));
                    }
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
        string base64RDB =
            "UkVESVMwMDEx+glyZWRpcy12ZXIFNy4yLjD6CnJlZGlzLWJpdHPAQPoFY3RpbWXCbQi8ZfoIdXNlZC1tZW3CsMQQAPoIYW9mLWJhc2XAAP/wbjv+wP9aog==";

        byte[] rdbBytes = Convert.FromBase64String(base64RDB);

        string header = $"${rdbBytes.Length}\r\n";

        client.Send(Encoding.UTF8.GetBytes(header));

        client.Send(rdbBytes);
    }
    private void EstablishReplicationHandshake(out Socket client , out string overflowCommand)
    {
        overflowCommand = null;

        // 3 handshake between slave and master
        var master = new TcpClient(
            configuration.MasterHost , configuration.MasterPort.Value
        );

        client = master.Client;
        //client.NoDelay = true;

        //PING command
        string pingCommand = "*1\r\n$4\r\nPING\r\n";

        if (SendAndVerify(client , pingCommand , "PONG"))
        {
            // REPLCONF command (listening-port)
            string replicaPort = configuration.Port.ToString();
            string replconf1 = $"*3\r\n$8\r\nREPLCONF\r\n$14\r\nlistening-port\r\n${replicaPort.Length}\r\n{replicaPort}\r\n";

            if (SendAndVerify(client , replconf1 , "OK"))
            {
                // REPLCONF command (capabilities)
                string replconf2 = "*3\r\n$8\r\nREPLCONF\r\n$4\r\ncapa\r\n$6\r\npsync2\r\n";

                if (SendAndVerify(client , replconf2 , "OK"))
                {
                    // PSYNC Command with <master replication id> and <offset>
                    string psyncCommand = "*3\r\n$5\r\nPSYNC\r\n$1\r\n?\r\n$2\r\n-1\r\n";

                    //SendAndVerify(client , psyncCommand , "FULLRESYNC");
                    SendAndVerifyWithOverflowCheck(client , psyncCommand , out overflowCommand);
                }
            }
        }
    }
    private bool SendAndVerify(Socket client , string command , string expected)
    {
        client.Send(Encoding.UTF8.GetBytes(command));

        byte[] buffer = new byte[1024];
        int bytesRead = client.Receive(buffer);
        string response = Encoding.UTF8.GetString(buffer , 0 , bytesRead);

        return response.Contains(expected);
    }
    private bool SendAndVerifyWithOverflowCheck(Socket client , string psyncCommand , out string overflowCommand)
    {
        overflowCommand = null;

        client.Send(Encoding.UTF8.GetBytes(psyncCommand));

        byte[] buffer = new byte[1024];
        int bytesRead = client.Receive(buffer);
        string response = Encoding.UTF8.GetString(buffer , 0 , bytesRead);

        // wait to read RDB File after PSYNC Command if it didn't return
        if (!response.Contains("REDIS"))
        {
            bytesRead = client.Receive(buffer);
            response = Encoding.UTF8.GetString(buffer , 0 , bytesRead);
        }

        //Console.WriteLine($"Last Responses: {response}");

        int commandStartIdx = response.IndexOf("*");
        if (commandStartIdx != -1)
        {
            overflowCommand = response.Substring(commandStartIdx);
            //Console.WriteLine($"Overflow Command: {overflowCommand}");
        }


        return true; // or false based on success
    }
}

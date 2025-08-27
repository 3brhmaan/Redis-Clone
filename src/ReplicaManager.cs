using codecrafters_redis.src.Commands;
using codecrafters_redis.src.Core;
using System.Net.Sockets;
using System.Text;

namespace codecrafters_redis.src;
public class ReplicaManager
{
    private readonly CommandExecutor commandExecutor;
    private readonly RedisServerConfiguration configuration;

    public ReplicaManager(CommandExecutor commandExecutor , RedisServerConfiguration configuration)
    {
        this.commandExecutor = commandExecutor;
        this.configuration = configuration;
    }

    public void EstablishHandshake(out Socket client , out string overflowCommand)
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
    public void StartListeningToMaster(Socket client , string overflowCommand)
    {
        string clientId = client.RemoteEndPoint?.ToString()!;

        if (overflowCommand is not null)
        {
            string overflowResponse = commandExecutor.Execute(overflowCommand , client);
            configuration.MasterReplOffset += overflowCommand.Length;

            if (overflowCommand.Contains("REPLCONF"))
            {
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
                    break; // Client disconnected
                }

                string tcpRequest = Encoding.UTF8.GetString(buffer , 0 , bytesRead);

                List<string> requests = ParseCommands(tcpRequest);


                foreach (string request in requests)
                {
                    //Console.WriteLine($"Readig {request.Length} bytes from {request}");
                    string response = commandExecutor.Execute(request , client);
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
    private List<string> ParseCommands(string request)
    {
        List<string> strRequests = request.Split("\r\n" , StringSplitOptions.RemoveEmptyEntries).ToList();
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

        return requests;
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

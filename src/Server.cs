using System.Net;
using System.Net.Sockets;
using System.Text;

// You can use print statements as follows for debugging, they'll be visible when running tests.
Console.WriteLine("Logs from your program will appear here!");

TcpListener server = new TcpListener(IPAddress.Any , 6379);
server.Start();


while (true)
{
    var client = server.AcceptSocket(); // wait for client

    var task = Task.Run(() => HandleClient(client));
}


void HandleClient(Socket client)
{
    // Handle a client in a loop to process multiple requests
    while (client.Connected)
    {
        byte[] buffer = new byte[1024];

        int bytesReaded = client.Receive(buffer);

        // check if connection was closed
        if (bytesReaded == 0)
        {
            break; // client disconneted
        }

        string request = Encoding.UTF8.GetString(buffer , 0 , bytesReaded);
        
        string response = ParseRedisCommand(request);

        client.Send(Encoding.UTF8.GetBytes(response));
    }
}

string ParseRedisCommand(string request)
{
    var requestParts = request.Split("\r\n" , StringSplitOptions.RemoveEmptyEntries);

    if (requestParts[2] == "PING")
        return "+PONG\r\n";
    else if(requestParts[2].ToLower() == "echo")
        return $"${requestParts[4].Length}\r\n{requestParts[4]}\r\n";
    else
        return "error";
}

/*
    PING
    *1\r\n$4\r\nPING\r\n
 */

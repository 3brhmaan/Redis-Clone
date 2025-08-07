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
    Dictionary<string, string> store = new();

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
        
        string response = ParseRedisCommand(request, store);

        client.Send(Encoding.UTF8.GetBytes(response));
    }
}

string ParseRedisCommand(string request, Dictionary<string , string> store)
{
    string separator = "\r\n";

    var commandParts = request.Split(separator , StringSplitOptions.RemoveEmptyEntries);

    var commandName = commandParts[2].ToUpper();
    var commandArguments = commandParts.Where(
        (value, i) => i >= 4 && i % 2 == 0
    ).ToArray();


    switch (commandName)
    {
        case "PING":
            return $"+PONG{separator}";
        case "ECHO":
            return $"${commandArguments[0].Length}{separator}{commandArguments[0]}{separator}";
        case "SET":
            store.Add(commandArguments[0], commandArguments[1]);
            return $"+OK{separator}";
        case "GET":
            string? value = store.GetValueOrDefault(commandArguments[0]);

            if (value is not null)
                return $"${value.Length}{separator}{value}{separator}";
            else
                return $"$-1{separator}";
        default:
            return "-ERR unknown command\r\n";
    }
}


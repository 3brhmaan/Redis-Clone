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

    // Handle each client in a loop to process multiple requests
    while (client.Connected)
    {
        try
        {
            byte[] buffer = new byte[1024];

            int bytesReaded = client.Receive(buffer);

            // check if connection was closed
            if(bytesReaded == 0)
            {
                break; // client disconneted
            }

            string request = Encoding.UTF8.GetString(buffer);
            Console.WriteLine($"Recived: {request} tmp");

            client.Send(Encoding.UTF8.GetBytes("+PONG\r\n"));
        }
        catch (Exception ex)
        {
            // Client disconnected or network error
            break;
        }

    }
}

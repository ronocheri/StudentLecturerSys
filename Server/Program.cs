using System;
using System.Threading;

class Program
{
    const int PORT_NO = 5000;
    const string SERVER_IP = "127.0.0.1";
    static void Main(string[] args)
    {
        Thread t = new Thread(delegate ()
        {
            // replace the IP with your system IP Address...
            Server.Server myserver = new Server.Server(SERVER_IP, PORT_NO);
        });
        t.Start();

        Console.WriteLine("Server Started...!");
    }
}

using System.Net.Sockets;
using System.Text;



using System;
using System.Net.Sockets;
using System.Threading;

class Program
{
    const int PORT_NO = 5000;
    const string SERVER_IP = "127.0.0.1";

    static void Main(string[] args)
    {
        new Thread(() =>
        {
            Thread.CurrentThread.IsBackground = false;
            Connect(); 
        }).Start();

        //new Thread(() =>
        //{
        //    Thread.CurrentThread.IsBackground = false;
        //    Connect();
        //}).Start();
    }

    static void Connect()
    {
        int queryID = 0, facultyID = 0;
        char res = 'Y';

        while (res == 'Y' || res == 'y')
        {
            queryID = 0; facultyID = 0;

            Console.WriteLine("Hello, lecturer! Welcome to our new system");
            while (queryID < 1 || queryID > 3)
            {
                Console.WriteLine("Please choose one of these queries:");
                Console.WriteLine("1- How many students applied to a certain faculty");
                Console.WriteLine("2- Average and standard deviation of the scores of the students who applied to this faculty");
                Console.WriteLine("3- The data of the 2 outstanding students who applied to this faculty");
                try
                {
                    queryID = Convert.ToInt32(Console.ReadLine());
                }

                catch (Exception ex)
                {
                    queryID = 0;
                }
            }
            //choose faculty
            while (facultyID < 1 || facultyID > 4)
            {
                Console.WriteLine("Please choose your faculty of undergraduate studies");
                Console.WriteLine("1- Exact sciences | 2- Social sciences | 3- Natural sciences | 4- Humanities");
                try
                {
                    facultyID = Convert.ToInt32(Console.ReadLine());

                }
                catch (Exception ex)
                {
                    facultyID = 0;
                }
            }

            //call the server
            string textToSend = "LecturerClient," + queryID + "," + facultyID;

            //---create a TCPClient object at the IP and port no.---
            TcpClient client = new TcpClient(SERVER_IP, PORT_NO);
            NetworkStream nwStream = client.GetStream();
            byte[] bytesToSend = ASCIIEncoding.ASCII.GetBytes(textToSend);

            //---send the text---
            Console.WriteLine("Sending : " + textToSend);
            nwStream.Write(bytesToSend, 0, bytesToSend.Length);

            //---read back the text---
            byte[] bytesToRead = new byte[client.ReceiveBufferSize];
            int bytesRead = nwStream.Read(bytesToRead, 0, client.ReceiveBufferSize);
            Console.WriteLine("Received : " + Encoding.ASCII.GetString(bytesToRead, 0, bytesRead));
            client.Close();

            //Console.WriteLine("Here are your picks- queryID:" + queryID + " ,facultyID:"+facultyID);
            res = 'a';
            while (res != 'Y' && res != 'y' && res != 'N' && res != 'n')
            {
                Console.WriteLine("Do you want to start again? (Y/N) \n");
                res = Console.ReadKey().KeyChar;
                Console.WriteLine();
            }

        }
    }

}
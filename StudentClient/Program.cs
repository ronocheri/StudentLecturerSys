
using System;
using System.Net.Sockets;
using System.Threading;
using System.Text;


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
    }

    static void Connect()
    {
        string studentId = "";
        int firstDegreeAnswer = 0, secondDegreeAnswer = 0;
        double avg = 0;
        char res = 'Y';

        while (res == 'Y' || res == 'y')
        {
            firstDegreeAnswer = 0; secondDegreeAnswer = 0; avg = 0; studentId = "";

            Console.WriteLine("Hello, student! Welcome to our new system");
            while (studentId == "")
            {
                Console.WriteLine("Please enter your ID");
                try
                {
                    studentId = Console.ReadLine();
                }
                catch (Exception ex)
                {
                    studentId = "";
                }
            }

            //faculty of undergraduate studies
            while (firstDegreeAnswer < 1 || firstDegreeAnswer > 4)
            {
                Console.WriteLine("Please choose your faculty of undergraduate studies");
                Console.WriteLine("1- Exact sciences | 2- Social sciences | 3- Natural sciences | 4- Humanities");
                try
                {
                    firstDegreeAnswer = Convert.ToInt32(Console.ReadLine());
                }
                catch (Exception ex)
                {
                    firstDegreeAnswer = 0;
                }
            }

            //average
            while (avg < 1 || avg > 100)
            {
                Console.WriteLine("Please enter your grade points average of first degree");
                try
                {
                    avg = Convert.ToDouble(Console.ReadLine());
                }
                catch (Exception ex)
                {
                    avg = 0;
                }

            }

            //faculty of second degree
            while (secondDegreeAnswer < 1 || secondDegreeAnswer > 4)
            {
                Console.WriteLine("Please choose your faculty of undergraduate studies");
                Console.WriteLine("1- Exact sciences | 2- Social sciences | 3- Natural sciences | 4- Humanities");
                try
                {
                    secondDegreeAnswer = Convert.ToInt32(Console.ReadLine());
                }
                catch (Exception ex)
                {
                    secondDegreeAnswer = 0;
                }
            }

            //Write to DB
            string textToSend = "StudentClient," + studentId + "," + firstDegreeAnswer + "," + avg + "," + secondDegreeAnswer;

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

            //option to run again
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
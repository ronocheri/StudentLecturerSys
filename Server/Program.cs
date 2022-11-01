using System;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Net.Sockets;
using System.Text;

class Program
{
    //connection string details
    const string datasource = @"DESKTOP-7IMBLDM";//server
    const string database = "StudentsPicks"; //database name
    const string connString = @"Data Source=" + datasource + ";Initial Catalog=" + database + ";Trusted_Connection=true";

    const int PORT_NO = 5000;
    const string SERVER_IP = "127.0.0.1";
    static void Main(string[] args)
    {

        //---listen at the specified IP and port no.---
        IPAddress localAdd = IPAddress.Parse(SERVER_IP);
        TcpListener listener = new TcpListener(localAdd, PORT_NO);
        Console.WriteLine("Server is listening...");
        listener.Start();

        //---incoming client connected---
        TcpClient client = listener.AcceptTcpClient();

        //---get the incoming data through a network stream---
        NetworkStream nwStream = client.GetStream();
        byte[] buffer = new byte[client.ReceiveBufferSize];

        //---read incoming stream---
        int bytesRead = nwStream.Read(buffer, 0, client.ReceiveBufferSize);

        //---convert the data received into a string---
        string dataReceived = Encoding.ASCII.GetString(buffer, 0, bytesRead);
        int startIndex = dataReceived.IndexOf(',');
        string fromClient= dataReceived.Substring(0,startIndex);
        string inputForQuery = dataReceived.Substring(startIndex + 1);

        Console.WriteLine("Received : " + dataReceived);

        if(fromClient== "StudentClient")
        {
            bool res = addDataToDB(inputForQuery); //write to DB

            //---write back to the client---
            Console.WriteLine("Sending back : " + res);

            // Encode the data string into a byte array.
            byte[] msg = Encoding.ASCII.GetBytes(res == true ? "Data was added" : "Insertion failed!");
            // Send the data through the socket.
            nwStream.Write(msg, 0, msg.Length);
        }
       else
        {
            string answer = "";

            startIndex = inputForQuery.IndexOf(',');
            int queryNum = Convert.ToInt32(inputForQuery.Substring(0, startIndex));
            int facultyID = Convert.ToInt32(inputForQuery.Substring(startIndex + 1));
            switch (queryNum)
            {
                case 1:
                    answer = "Number of students applied to this faculty: " + getNumOfAppliedStudentsPerFaculty(facultyID);
                    break;
                case 2:
                    answer = "Average and standard deviation of the students who applied to this faculty: (AVG, STD) " + getAvgStandartDivisionPerFaculty(facultyID);
                    break;

                case 3:
                    string res = getTopTwoStudentnsPerFaculty(facultyID);
                    answer = "The data of the 2 outstanding students who applied to this faculty: (ID,AVG):" + "\n" + (res != "" ? res : "None");
                    break;
            }

            //---write back to the client---
            Console.WriteLine("Sending back : " + answer);

            // Encode the data string into a byte array.
            byte[] msg = Encoding.ASCII.GetBytes(answer);
            // Send the data through the socket.
            nwStream.Write(msg, 0, msg.Length);
        }


        client.Close();
        listener.Stop();
        Console.ReadLine();

    }

    //Query 1 - Number of students applications per faculty
    private static int getNumOfAppliedStudentsPerFaculty(int facultyID)
    {
        string queryString =
            "Select count(distinct studentId) from StudentsPicks where facultySecDegId = " + facultyID;

        using (SqlConnection connection =
                   new SqlConnection(Program.connString))
        {
            SqlCommand command =
                new SqlCommand(queryString, connection);
            connection.Open();

            SqlDataReader reader = command.ExecuteReader();

            StringBuilder res = new StringBuilder(); //for resukt
            // Call Read before accessing data.
            while (reader.Read())
            {
                res.AppendLine(ReadSingleRow((IDataRecord)reader));
            }

            // Call Close when done reading.
            reader.Close();
            return Convert.ToInt32(res.ToString());

            //int num=command.ExecuteNonQuery(); //execute the Query
            //return num;

        }
    }

    //Query 2 - Average and standard deviation of the scores of the students who applied to this faculty
    private static string getAvgStandartDivisionPerFaculty(int facultyID)
    {
        string queryString =
            "Select AVG (avg),ISNULL(STDEV(avg), 0 ) from StudentsPicks where facultySecDegId = " + facultyID;

        using (SqlConnection connection =
                   new SqlConnection(Program.connString))
        {
            SqlCommand command =
                new SqlCommand(queryString, connection);
            connection.Open();

            SqlDataReader reader = command.ExecuteReader();
            StringBuilder res = new StringBuilder(); //for resukt
            // Call Read before accessing data.
            while (reader.Read())
            {
                res.AppendLine(ReadSingleRow((IDataRecord)reader));
            }

            // Call Close when done reading.
            reader.Close();
            return res.ToString();

        }
    }

    //Query 3 - The data of the 2 outstanding students who applied to this faculty
    private static string getTopTwoStudentnsPerFaculty(int facultyID)
    {
        string queryString =
            "Select top 2 studentId,avg from StudentsPicks where facultySecDegId =" + facultyID + "Order by avg DESC";

        using (SqlConnection connection =
                   new SqlConnection(Program.connString))
        {
            SqlCommand command =
                new SqlCommand(queryString, connection);
            connection.Open();

            SqlDataReader reader = command.ExecuteReader();

            StringBuilder res = new StringBuilder(); //for resukt
            // Call Read before accessing data.
            while (reader.Read())
            {
                res.AppendLine(ReadSingleRow((IDataRecord)reader));
            }

            // Call Close when done reading.
            reader.Close();
            return res.ToString();

        }
    }

    //read a row
    private static string ReadSingleRow(IDataRecord dataRecord)
    {
        StringBuilder res = new StringBuilder();
        int count = dataRecord.FieldCount;
        for (int i = 0; i < count; i++)
        {
            res.Append(dataRecord[i].ToString().Trim() + " ");
        }
        return res.ToString();
    }


    //Write to DB
    private static bool addDataToDB(string input)
    {
        //create instanace of database connection
        SqlConnection conn = new SqlConnection(connString);

        try
        {
            //open connection
            conn.Open();

            //create a new SQL Query using StringBuilder
            StringBuilder queryString = new StringBuilder();
            queryString.Append("INSERT INTO StudentsPicks (studentId, facultyId, avg, facultySecDegId) VALUES ");
            queryString.Append("(" + input + ")");

            string sqlQuery = queryString.ToString();
            using (SqlCommand command = new SqlCommand(sqlQuery, conn)) //pass SQL query created above and connection
            {
                command.ExecuteNonQuery(); //execute the Query
                Console.WriteLine("Query Executed.");
                return true;
            }
        }
 
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            return false;
        }
        
    }


}
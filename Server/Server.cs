using System;
using System.Data.SqlClient;
using System.Data;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;


namespace Server
{

    public class Server
    {
        //connection string details
        const string datasource = @".\SQLEXPRESS";//server
        const string database = "StudentsPicks"; //database name
        const string connString = @"Data Source=" + datasource + ";Initial Catalog=" + database + ";Trusted_Connection=true";

        TcpListener server = null;
        public Server(string ip, int port)
        {
            IPAddress localAddr = IPAddress.Parse(ip);
            server = new TcpListener(localAddr, port);
            server.Start();
            StartListener();
        }

        public void StartListener()
        {
            try
            {
                while (true)
                {
                    Console.WriteLine("Waiting for a connection...");
                    TcpClient client = server.AcceptTcpClient();
                    Console.WriteLine("Connected!");

                    Thread t = new Thread(new ParameterizedThreadStart(HandleDeivce));
                    t.Start(client);
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
                server.Stop();
            }
        }

        public void HandleDeivce(Object obj)
        {
            TcpClient client = (TcpClient)obj;
            var stream = client.GetStream();
            string imei = String.Empty;

            string data = null;
            Byte[] bytes = new Byte[256];
            int i;
            try
            {
                while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                {
                    //Read
                    data = Encoding.ASCII.GetString(bytes, 0, i);

                    //---convert the data received into a string---
                    int startIndex = data.IndexOf(',');
                    string fromClient = data.Substring(0, startIndex);
                    string inputForQuery = data.Substring(startIndex + 1);

                    Console.WriteLine("Received : " + data);



                    //Write

                    if (fromClient == "StudentClient")
                    {
                        bool res = addDataToDB(inputForQuery); //write to DB

                        //---write back to the client---
                        Console.WriteLine("Sending back : " + res);

                        // Encode the data string into a byte array.
                        byte[] msg = Encoding.ASCII.GetBytes(res == true ? "Data was added" : "Insertion failed!");
                        // Send the data through the socket.
                        stream.Write(msg, 0, msg.Length);
                    }
                    else
                    {
                        string answer = "";

                        startIndex = inputForQuery.IndexOf(',');
                        int queryNum = Convert.ToInt32(inputForQuery.Substring(0, startIndex)); //extract the query number
                        int facultyID = Convert.ToInt32(inputForQuery.Substring(startIndex + 1)); //extract the faculty id
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
                        stream.Write(msg, 0, msg.Length);
                    }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: {0}", e.ToString());
                client.Close();
            }
        }

        //Query 1 - Number of students applications per faculty
        private static int getNumOfAppliedStudentsPerFaculty(int facultyID)
        {
            string queryString =
                "Select count(distinct studentId) from StudentsPicks where facultySecDegId = " + facultyID;

            using (SqlConnection connection =
                       new SqlConnection(connString))
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

            }
        }

        //Query 2 - Average and standard deviation of the scores of the students who applied to this faculty
        private static string getAvgStandartDivisionPerFaculty(int facultyID)
        {
            string queryString =
                "Select AVG (avg),ISNULL(STDEV(avg), 0 ) from StudentsPicks where facultySecDegId = " + facultyID;

            using (SqlConnection connection =
                       new SqlConnection(connString))
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
                       new SqlConnection(connString))
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
 
}

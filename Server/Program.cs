using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;

class Program
{
    //connection string details
    static string datasource = @"DESKTOP-7IMBLDM";//server
    static string database = "StudentsPicks"; //database name
    static string connString = @"Data Source=" + datasource + ";Initial Catalog=" + database + ";Trusted_Connection=true";
    static void Main(string[] args)
    {

        //int queryNum = Convert.ToInt32(args[0]);
        //int facultyID = Convert.ToInt32(args[1]);

        int queryNum = 3;
        int facultyID = 2;
        string answer = "";

        //getAllStudentsPicksData(connString); //All Data (TEST)

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


        Console.WriteLine(answer);

    }

    //read all data
    private static void getAllStudentsPicksData()
    {
        string queryString =
            "Select * from StudentsPicks";

        using (SqlConnection connection =
                   new SqlConnection(Program.connString))
        {
            SqlCommand command =
                new SqlCommand(queryString, connection);
            connection.Open();

            SqlDataReader reader = command.ExecuteReader();

            // Call Read before accessing data.
            while (reader.Read())
            {
                ReadSingleRow((IDataRecord)reader);
            }

            // Call Close when done reading.
            reader.Close();
        }
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


}

int queryID=0, facultyID = 0;
double avg = 0;

Console.WriteLine("Hello, lecturer! Welcome to our new system");
while (queryID < 1 || queryID > 3)
{
    //if (queryID == -1)
    //    break;

    Console.WriteLine("Please enter your query id");
    Console.WriteLine("1- How many students applied to a certain faculty");
    Console.WriteLine("2- Average and standard deviation of the scores of the students who applied to this faculty");
    Console.WriteLine("3- The data of the 2 outstanding students who applied to this faculty");
    try
    {
        queryID = Convert.ToInt32(Console.ReadLine());

        //choose faculty
        while (facultyID < 1 || facultyID > 4)
        {
            Console.WriteLine("Please choose your faculty of undergraduate studies");
            Console.WriteLine("1- Exact sciences /2- Social sciences /3- Natural sciences /4- Humanities");
            try
            {
                facultyID = Convert.ToInt32(Console.ReadLine());

                //call the server

                Console.WriteLine("Here are your picks- queryID:" + queryID + " ,facultyID:"+facultyID);

            }
            catch (Exception ex)
            {
                facultyID = 0;
            }

        }

    }
    catch (Exception ex)
    {
        queryID = 0;
    }
}
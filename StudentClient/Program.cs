using System.Diagnostics;

string studentId = "", FirstDegreefaculty = "", secondDegreefaculty = "";
int firstDegreeAnswer = 0, secondDegreeAnswer = 0;
double avg = 0;

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

Console.WriteLine("Hello, student! Welcome to our new system");

//faculty of undergraduate studies
while (firstDegreeAnswer < 1 || firstDegreeAnswer > 4)
{
    Console.WriteLine("Please choose your faculty of undergraduate studies");
    Console.WriteLine("1- Exact sciences /2- Social sciences /3- Natural sciences /4- Humanities");
    try
    {
        firstDegreeAnswer = Convert.ToInt32(Console.ReadLine());
    }
    catch (Exception ex)
    {
        firstDegreeAnswer = 0;
    }
}

switch (firstDegreeAnswer)
{
    case 1:
        FirstDegreefaculty = "Exact sciences";
        break;
    case 2:
        FirstDegreefaculty = "Social sciences";
        break;
    case 3:
        FirstDegreefaculty = "Natural sciences";
        break;
    case 4:
        FirstDegreefaculty = "Humanities";
        break;
    default: break;
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
    Console.WriteLine("1- Exact sciences /2- Social sciences /3- Natural sciences /4- Humanities");
    try
    {
        secondDegreeAnswer = Convert.ToInt32(Console.ReadLine());
    }
    catch (Exception ex)
    {
        secondDegreeAnswer = 0;
    }
}

switch (secondDegreeAnswer)
{
    case 1:
        secondDegreefaculty = "Exact sciences";
        break;
    case 2:
        secondDegreefaculty = "Social sciences";
        break;
    case 3:
        secondDegreefaculty = "Natural sciences";
        break;
    case 4:
        secondDegreefaculty = "Humanities";
        break;
    default: break;
}


Console.WriteLine("Here are your picks (id:" + studentId + ") :");

Console.WriteLine("FirstDegreefaculty: " + FirstDegreefaculty + ", avg: " + avg + " , secondDegreefaculty: " + secondDegreefaculty);

//Write to DB - To Do

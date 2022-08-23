namespace Quizzes;

public class Quiz4 : RunnableQuiz
{
  protected override void RunQuizSnippet()
  {
    var x = 0;
    x = x++;
    Console.WriteLine(x);
  }

  public static void Quiz4a()
  {
    var x = 0;
    var y = x++;
    Console.WriteLine($"x={x}; y={y}");
  }

  public struct Integer
  {
    public int i;

    public static Integer operator ++(Integer i)
    {
      i.i++;
      return i;
    }
  }

  public static void Quiz4b()
  {
    Integer a = new Integer();
    a = a++;
    Console.WriteLine($"Value of i from struct Integer: {a.i}");
  }
}
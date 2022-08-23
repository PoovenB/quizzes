namespace Quizzes;

public class Quiz1: RunnableQuiz
{
  public static void Quiz1a()
  {
    int? x = 125;
    int? y = null;
    int? z = x + y;
    Console.WriteLine(z);
  }

  protected override void RunQuizSnippet()
  {
    int? i = null;
    i++;
    Console.WriteLine(i);
  }
}
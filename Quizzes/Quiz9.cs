namespace Quizzes;

public class Quiz9 : RunnableQuiz
{
  public static bool First()
  {
    Console.Write("First ");
    return false;
  }

  public static bool Second()
  {
    Console.Write("Second ");
    return true;
  }

  protected override void RunQuizSnippet()
  {
    var boolAnd = First() && Second();
    var bitwiseAnd = First() & Second();
    Console.WriteLine($"boolAnd: {boolAnd}, bitwiseAnd: {bitwiseAnd}");
  }
}
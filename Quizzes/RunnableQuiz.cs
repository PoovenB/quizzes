namespace Quizzes;

public abstract class RunnableQuiz
{
  public void Run()
  {
    Console.WriteLine($"{GetType().Name} Output:");
    try
    {
      RunQuizSnippet();
    }
    catch (Exception e)
    {
      Console.WriteLine($"Exception was thrown of type {e.GetType().Name}: {e.Message}");
    }
    Console.WriteLine("================================");
  }

  protected abstract void RunQuizSnippet();
}
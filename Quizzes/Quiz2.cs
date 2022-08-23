namespace Quizzes;

public class Quiz2 : RunnableQuiz
{
  abstract class Base
  {
    public virtual void Something(string s = "base")
    {
      Console.WriteLine("base " + s);
    }
  }

  class Derived : Base
  {
    public override void Something(string s = "derived")
    {
      Console.WriteLine("derived " + s);
    }
  }

  protected override void RunQuizSnippet()
  {
    Base b = new Derived();
    b.Something();
  }
}
using System.Collections;
using System.Linq;

namespace Quizzes;

public class Quiz5: RunnableQuiz
{
  class Parent
  {
    public virtual void Foo(int x)
    {
      Console.WriteLine("a");
    }
  }

  class Child : Parent
  {
    public override void Foo(int x)
    {
      Console.WriteLine("b");
    }

    public void Foo(double y)
    {
      Console.WriteLine("c");
    }
  }

  protected override void RunQuizSnippet()
  {
    Child c = new Child();
    c.Foo(10);
  }

  public static void Quiz5a()
  {
    Parent p = new Child();
    p.Foo(10);
  }

  static int GetCount<T>(ICollection<T> collection) => collection.Count;
  static int GetCount(ICollection collection) => collection.Count;

  public static void Quiz5b()
  {
    //GetCount(new int[10]);
  }
}
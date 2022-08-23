namespace Quizzes;

public class Quiz10 : RunnableQuiz
{
  interface IGet { int Value { get; } }
  interface ISet { int Value { set; } }
  interface IBoth : IGet, ISet { }

  class Both : IBoth
  {
    public int Value { get; set; }
  }

  protected override void RunQuizSnippet()
  {
    //IBoth b = new Both();
    //b.Value = 10;
    //Console.WriteLine(b.Value);
  }

  public static void Quiz10a()
  {
    Both b = new Both();
    b.Value = 10;

    Console.WriteLine(b.Value);
  }

  public static void Quiz10b()
  {
    IBoth b = new Both();
    ((ISet)b).Value = 10;
    Console.WriteLine(((IGet)b).Value);
  }

  class BothB : IBoth
  {
    private int value;

    int IGet.Value { get { return value; } }
    int ISet.Value { set { this.value = value; } }
  }
}
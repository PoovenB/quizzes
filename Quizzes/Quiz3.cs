namespace Quizzes;

public class Quiz3 : RunnableQuiz
{
  protected override void RunQuizSnippet()
  {
    IList<int> myList = new int[] { 1, 2, 4 };
    myList.Add(5);
  }
}
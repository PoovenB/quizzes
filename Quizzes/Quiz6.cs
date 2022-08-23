namespace Quizzes;

public class Quiz6: RunnableQuiz
{
  public struct User
  {
    public string FirstName { get; set; }
    public string LastName { get; set; }
  }

  protected override void RunQuizSnippet()
  {
    var userList = new Dictionary<User, int>();
    var voldemort = new User { FirstName = "Tom", LastName = "Riddle" };
    userList[voldemort] = 1;
    voldemort.FirstName = "Lord";
    Console.WriteLine(userList[voldemort]);
  }
}
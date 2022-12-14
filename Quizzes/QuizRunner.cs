using System.Reflection;
using System.Text.RegularExpressions;

namespace Quizzes;

public static class QuizRunner
{
  private static readonly Regex quizNumber = new Regex(@"Quiz(\d+)", RegexOptions.Compiled);

  /// <summary>
  /// Gets all types defined in the assembly that implement the specified
  /// base type.
  /// </summary>
  /// <typeparam name="TBaseType">The base type.</typeparam>
  /// <param name="assembly">The assembly containing the types.</param>
  /// <returns>The types that implement the base type contained in the
  /// assembly.</returns>
  public static IEnumerable<Type> GetTypesOf<TBaseType>(this Assembly assembly) =>
    assembly.GetTypes().Where(type =>
      !type.IsAbstract &&
      typeof(TBaseType).IsAssignableFrom(type));

  /// <summary>
  /// Finds, and instantiates all implementations of the specified type
  /// in the project. The default constructor is expected.
  /// </summary>
  public static IEnumerable<TInstance> CreateInstances<TInstance>(this Assembly assembly)
      => assembly.GetTypesOf<TInstance>().Select(type => (TInstance)Activator.CreateInstance(type));

  private static int GetQuizNumber(this string quizName)
    => int.Parse(quizNumber.Match(quizName).Groups[1].Value);

  public static void RunAllQuizzes()
  {
    var quizzes = Assembly
      .GetExecutingAssembly()
      .CreateInstances<RunnableQuiz>()
      .OrderBy(quiz => quiz.GetType().Name.GetQuizNumber());
    foreach (var quiz in quizzes)
    {
      quiz.Run();
    }
  }
}
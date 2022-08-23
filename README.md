- [Quiz 1: Nullables](#quiz-1-nullables)
  - [Question](#question)
  - [Answer](#answer)
- [Quiz 2: Optional Parameters](#quiz-2-optional-parameters)
  - [Question](#question-1)
  - [Answer](#answer-1)
- [Quiz 3: Liskov Substitution](#quiz-3-liskov-substitution)
  - [Question](#question-2)
  - [Answer](#answer-2)
- [Quiz 4: Post-Increment](#quiz-4-post-increment)
  - [Question](#question-3)
  - [Answer](#answer-3)
- [Quiz 5: Overloads](#quiz-5-overloads)
  - [Question](#question-4)
  - [Answer](#answer-4)
- [Quiz 6: Objects](#quiz-6-objects)
  - [Answer](#answer-5)
- [Quiz 7: Dictionaries](#quiz-7-dictionaries)
  - [Question](#question-5)
  - [Answer](#answer-6)
- [Quiz 8: HTTPS](#quiz-8-https)
  - [Question](#question-6)
  - [Answer](#answer-7)
- [Quiz 9: Bitwise operations](#quiz-9-bitwise-operations)
  - [Question](#question-7)
  - [Answer](#answer-8)
- [Quiz 10: Interfaces](#quiz-10-interfaces)
  - [Question](#question-8)
  - [Answer](#answer-9)

## Quiz 1: Nullables

### Question

What's the output of this snippet?

    int? i = null;
    i++;
    Console.WriteLine(i);

1. *No output*/*blank line*
2. `InvalidOperationException` is thrown
3. `NullReferenceException` is thrown
4. `0`
5. `1`

### Answer

**1**

`int?` is syntactic sugar for `Nullable<int>`. Let's have a look at the declaration:

    public struct Nullable<T> where T : struct

All `struct`s are value types, meaning they can never be `null`. Therefore, line 2 does not throw an exception; if some kind of exception were to be thrown, it would have been an `InvalidOperationException` (which is thrown if you access the `Value` property when a value hasn't been set, that is `i.Value` would have thrown an exception).

Surely we'd be accessing the `Value` property of the nullable type in line 2? Nullable types were overloaded so that mathematical operations returned `null` if no value was set. So this code is also perfectly safe:

    int? x = 125;
    int? y = null;
    int? z = x + y;

The variable `z` would simply be `null`. That is, `null` is preserved. Which is what you'd probably program if you were performing the `null` checks manually anyway. In our case, the compile generates code like this:

    i = i.HasValue ? new int?(i.GetValueOrDefault() + 1) : new int?();

I'm sharing this bit only to emphasis another point: nullable types are immutable. Anyway, when we try to output the value of `i`, `null` is passed to `WriteLine` and so nothing is output (well not quite: a new line is printed, but that wasn't the point of the question).

There isn't an overload for `Nullable<int>` so it gets cast to an object. At this stage, boxing will have to happen (value type to reference type). For nullable types, only the underlying type gets boxed. In the case when the value is `null`, the boxing results in a `null`. This is exactly what we expect, it's just that the reasoning behind it might not necessarily be straightforward.

## Quiz 2: Optional Parameters

### Question

Bill Wagner (author of Effective C#) recommends using optional parameters to minimize method overloads (item 10 in his book). As a side note, Google's coding standards [recommend not using method overloads](https://google.github.io/styleguide/cppguide.html#Function_Overloading).

Consider:

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

For the following code snippet, what gets output?

    Base b = new Derived();
    b.Something();

1. `base base`
2. `base derived`
3. `derived derived`
4. `derived base`

### Answer

**4**

To get this correct, you'd have to understand how optional parameters are implemented. There are a few possibilities:

1. The call site[^1] is unchanged, and when the method receives fewer parameters, the CRL understands that the default value of the optional parameter(s) should be used.
2. At runtime, reflection is used at the call site to determine the default value(s) of the optional parameter(s), and those get passed through to the method.
3. The compiler generates method overloads and chains the calls accordingly. For example:

        public virtual void Something(string s)
        {
          Console.WriteLine("base " + s);
        }

        // And then a generated method for the optional parameter case:
        public virtual void Something()
        {
          Something("base");
        }

4. The call site is updated so that the default value of the optional parameter is inserted. For example:
`b.Something();` becomes `b.Something("base");`

[^1]: The call site refers to the place in code in which you invoke/call the method. For example: `b.Something();`

The C# language team implemented option 4: the call site is updated with the default value of the optional parameter. This is done at compiler-time. In item 10 of Effective C#, Bill Wagner explains this in more detail, but for now you can settle for my explanation.

At compiler time, the compiler will never have information about the runtime type of the object (since objects can be created conditionally). The compiler can only make decisions based on the type of the variable (that is, it's using static binding). In our example, the compile time type of `b` is `Base`, therefore the compiler looks at the value of the optional parameter in the `Base` class. In this context, the compiler could figure out that `b` is actually of type `Derived`, but then we'd have complicated rules regarding this behaviour – also, static binding is pretty standard (also used for polymorphism).

When the method `Something` is invoked, usual polymorphism takes place (at runtime) so the implementation in the `Derived` class is called. Thus the output is `derived base`.

If we had:

    Derived b = new Derived();
    b.Something();

The output would have been `derived derived`. If you tried this on Visual Studio, you would have noticed a warning. In generally, it's not great for derived classes to implement different default values for virtual methods (because confusing things like this will happen).

## Quiz 3: Liskov Substitution

### Question

What happens when the following code is compiled/executed?

    IList<int> myList = new int[] { 1, 2, 4 };
    myList.Add(5);

1. `5` is added to the list
2. Nothing happens
3. An exception is thrown
4. The code snippet does not compile

### Answer

**3**

The code does compile because all primitive arrays implement `System.Array` which has the following declaration:

    public abstract class Array : ICloneable, IList, ICollection, IEnumerable, IStructuralComparable, IStructuralEquatable


However, we know that `int[]` is a fixed length array. Therefore an exception will be thrown. This is an example of the *Liskov Substitution* principle violation. It should be noted that `IList` has two properties eluding to this behaviour:

1. IsFixedSize
2. IsReadOnly

And of course, if either of those are `true`, `Add` will fail. Thus an `NotSupportedException` will be thrown. It's a great reminder that we should always try to program against an easy-to-use interface, and that in general we have to abiding by the requirements of that interface: in this case, if I accept an `IList`, I should be sure that any `Add` operations are only done if `IsFixedSize` and `IsReadOnly` aren't true.

## Quiz 4: Post-Increment

### Question

Given the following code snippet, what is displayed on the console?

    var x = 0;
    x = x++;
    Console.WriteLine(x);

### Answer

The answer is `0`. Let's get the easy case done first; if we rather had:

    var x = 0;
    var y = x++;
    Console.WriteLine(x);

Then the output would have been `1`, and the value of `y` would have been `0`. That is, the post-increment works exactly as you'd expect:

- First return the value (which is effectively creating a copy)
- Then increment the variable by 1

Pre-increment is even easier, `++x` means:

- Increment the value of the variable by 1
- Then return the value of the variable

C# let's your define the pre-increment operator; consider:

    public struct Integer
    {
      public int i;
      
      public static Integer operator++(Integer i)
      {
        i.i++;
        return i;
      }
    }

Then:

    Integer a = new Integer();
    a = a++;
    Console.WriteLine(a.i);

Also results in a `0`. C# does not allow us to implement the post-increment operator. Instead, the compile generates the post-increment operation for us that will:

- Create a copy, and return the copy
- Call the pre-increment operator on the original object

Therefore this wouldn't work well for reference types.

So what's going on in our case? The expression `x++` must be fully evaluated before the assignment can be made. What's weird in this case is that the expression has a side effect, but that side effect must be resolved before the assignment. And in our case, that assignment must be a `0`. It's also interesting to note that C# will always evaluate expressions from left to right[^2] (operator precedence still applies but for example `a * b * c` will be evaluated as `a * b`, and then the `result * c`).

[^2]: The compiler is allowed to change the order if it can guarantee that the result and accompanied side effects are unchanged.

Let's have a look at the IL:

| Code         | IL                                                                                                            | IL Description                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                       |
| ------------ | ------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| `var x = 0;` | <ol><li>ldc.i4.0</li><li>stloc.0</li></ol>                                                                    | <ol><li>Push 0 onto the evaluation stack</li><li>Pop value on evaluation stack and store it in *x*.</li></ol>                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        |
| `x = x++;`   | <ol><li>ldloc.0</li><li>dup</li><li>ldc.i4.1</li><li>add</li><li>stloc.0</li><li>stloc.0</li><li>stloc.0</li> | <ol><li>Load *x* onto the evaluation stack</li><li>Duplicate the last added value on the evaluation stack and push it on the stack: that is, create a copy of *x*.</li><li>Push 1 onto the evaluation stack: since we’re incrementing *x* by 1.</li><li>Pop and add last two values on the evaluation stack, and push the result on the evaluation stack. At this stage, we have 1 (the result of the increment), and 0 (the previous value) on the evaluation stack.</li><li>Pop and store the result in *x* (at this stage the expression has fully evaluated, and *x* is 1)</li><li>Pop and store the result in *x* (now we’re ready to perform the assignment, so *x* becomes 0) |


Of course the compiler will optimize most of this away, [constant folding](https://en.wikipedia.org/wiki/Constant_folding) for the win 😊

## Quiz 5: Overloads

### Question

Consider the following classes:

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

What will the following code snippet output?

    Child c = new Child();
    c.Foo(10);

1. `a`
2. `b`
3. `c`
4. The code does not compile.

### Answer

**3**

In effort to reduce the [fragile base class](https://en.wikipedia.org/wiki/Fragile_base_class) problem, the designers decided that if a candidate method exists on the **type**, then the base class methods are no longer considered. Eric Lippert [explains it here](https://blogs.msdn.microsoft.com/ericlippert/2007/09/04/future-breaking-changes-part-three/) if you're interested in understanding this decision better. The rules around this are defined in the [language specification](https://www.microsoft.com/en-us/download/details.aspx?id=7029), section 7.5.3. Thus the output is `c`.

This also means that if we rather had:

    Parent p = new Child();
    p.Foo(10);

Then we'd get `b` as the output. Let's look at another interesting example:


    static int GetCount<T>(ICollection<T> collection) => collection.Count;
    static int GetCount(ICollection collection) => collection.Count;

However, since most types that implement` ICollection` also implement `ICollection<T>`, this code won’t compile:

    GetCount(new int[10]);

## Quiz 6: Objects

Is the following statement true or false? In C#, all types derive from the Object class.

### Answer

**False**

Not all types inherit from `Object`. I stumbled upon this while reading yet another [Eric Lippert blog post](https://blogs.msdn.microsoft.com/ericlippert/2009/08/06/not-everything-derives-from-object/). Here's a summary:

- Interfaces can't inherit from anything other than another interface. However interfaces can be converted to an object.
- [Pointer types](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/unsafe-code-pointers/pointer-types) don't inherit, and can't be converted to an object.

## Quiz 7: Dictionaries

### Question

Consider the following `struct`:

    public struct User
    {
      public string FirstName { get; set; }
      public string LastName { get; set; }
    }

Given the following code snippet, what gets output?

    var userList = new Dictionary<User, int>();
    var voldemort = new User { FirstName = "Tom", LastName = "Riddle" };
    userList[voldemort] = 1;
    voldemort.FirstName = "Lord";
    Console.WriteLine(userList[voldemort]);

1. 1
2. Tom
3. Riddle
4. Lord
5. Voldemort
6. The key is not found, so an exception is thrown

### Answer

**6**

The *Dictionary* in C# is implemented using a [hash table](https://en.wikipedia.org/wiki/Hash_table), which means that:

1. The key (in our case, the `User` object), must be converted to an integer. This is usually done via a [hash function](https://en.wikipedia.org/wiki/Hash_function).
2. The integer is mapped to an index of an array/bucket which stores the actual data.

The following methods are used to get this done:

- [GetHashCode](https://msdn.microsoft.com/en-us/library/system.object.gethashcode(v=vs.110).aspx): The object that's used as a key is responsible for implementing a hash function, and returning the result.
- [Equals](https://msdn.microsoft.com/en-us/library/bsc2ak47(v=vs.110).aspx): In general, the hash code is *unbound*, but has to map in to an index of a fixed sized structure. Therefore the [modulo](https://en.wikipedia.org/wiki/Modulo_operation) is taken, allowing two keys to resolve to the same index. Thus the equality of the key has to be verified.

For reference types, the default hash function is based off the memory address. It's quick, but means that two separate objects with the same content will generate different hash codes. Value types are more interesting:

- The primary requirement for a hash function is that the same value returns the same code.
- Since value types are stored on the stack (which gets reused), the memory address can't simply be used.
- The CLR therefore uses the raw bytes of the struct in memory, or follows the references used in the struct to determine the hash code. 

This means that the hash code when the struct was initialized, and then when the struct was subsequently modified, are different. Therefore, the dictionary lookup will fail, resulting in an exception. If the object was instead a reference type, the memory address wouldn't have changed, and the lookup would have succeeded.

The moral of the story: dictionaries are great, but be careful when you use them. Structs also make more sense when they are immutable. In *Effective C#*, Bill Wagner discusses this in *Item 7: Understand the pitfalls of GetHashCode()*.

## Quiz 8: HTTPS

### Question

Consider websites that don't display sensitive information, like http://www.google.cn. 

Are HTTP websites safe to use?

1. Yes
2. No

### Answer

**2**

Let's consider a website that neither has user, nor any other sensitive information. If a [man-in-the-middle attack](https://en.wikipedia.org/wiki/Man-in-the-middle_attack) was done, what's the worst that could happen?

1.  No information is lost, because there's nothing to steal[^3].
2.  The content could be tampered.
3.  Some other nefarious thing that you haven't considered, or doesn't yet exist[^4]. That is, you're exposing an [attack surface](https://en.wikipedia.org/wiki/Attack_surface) by not following best-practice[^5].

It's the second case that I wanted to highlight: the point of HTTPS isn't just to protect sensitive information. HTTPS also allows the browser to verify the source of the data.

In Chrome, if you click on the lock icon in the omnibox, and access the site *Certificate*, you'll see that the certificate lists the site it's associated with. If we can verify the source, and if the content is encrypted, then attack vector for tampered content is greatly reduced.

So in conclusion, you should use HTTPS for all traffic, regardless of the purpose of your site. See here for details, and references of tampered content: <https://doesmysiteneedhttps.com/>. My favourite was this one: <https://twitter.com/konklone/status/598696478018666496>

[^3]: The act of viewing the website might facilitate [social hacking](https://en.wikipedia.org/wiki/Social_hacking), or expose other [trackable information](https://amiunique.org/).
[^4]: For example, hyper-threading as been around for a while, but [a vulnerability was recently found](https://www.tomshardware.com/news/intel-disable-hyper-threading-spectre-attack,39333.html).
[^5]: For example, [Google](https://developers.google.com/web/fundamentals/security/encrypt-in-transit/why-https), and the [UK National Cyber Security Centre](https://www.ncsc.gov.uk/blog-post/serve-websites-over-https-always) recommend using HTTPS for all traffic.

## Quiz 9: Bitwise operations

### Question

Consider the following C# code:

    public static bool First()
    {
      Console.WriteLine("First");
      return false;
    }

    public static bool Second()
    {
      Console.WriteLine("Second");
      return true;
    }

What's the output of the following code snippet?

    var boolAnd = First() && Second();
    var bitwiseAnd = First() & Second();
    Console.WriteLine($"boolAnd: {boolAnd}, bitwiseAnd: {bitwiseAnd}");

1. First First Second boolAnd: False, bitwiseAnd: False
2. First Second First Second boolAnd: False, bitwiseAnd: False
3. First First boolAnd: False, bitwiseAnd: True
4. First Second First Second boolAnd: False, bitwiseAnd: True

### Answer

**1**

When the bitwise `and` (`&`) or `or` (`|`) operations are applied in a Boolean expression, both operands are evaluated. When the logical `and` (`**&&**`) or `or` (`**||**`) operations are applied to Boolean expressions, [short-circuiting](https://en.wikipedia.org/wiki/Short-circuit_evaluation) is applied (the second operand is only evaluated if necessary).

However, short-circuiting doesn't come for free: the compiler has to emit code that only evaluates the second expression if necessary. Branching tends to slow down the processor, so in some cases the bitwise operations are faster than the logical operations. However, these considerations are examples of premature optimization, and in general we should code for readability (or equivalently maintainability). You can read more about this here: <https://ericlippert.com/2015/11/02/when-would-you-use-on-a-bool/>

## Quiz 10: Interfaces

### Question

Consider the following code:

    interface IGet { int Value { get; } }
    interface ISet { int Value { set; } }
    interface IBoth : IGet, ISet { }
  
    class Both : IBoth 
    {
      public int Value { get; set; }
    }

What is the output for the following code snippet?

    IBoth b = new Both();
    b.Value = 10;
    Console.WriteLine(b.Value);

1. An exception is thrown
2. 10
3. The code doesn't compile
4. No output/blank line

### Answer

**3**

The code snippet won't compile because `IBoth` has two definitions for `Value`, one from `IGet`, and the other from `ISet`. That is, `b.Value` ambiguous until you look at its context (are you setting, or getting?), and the compiler tends to avoid contextual clues unless the benefit is substantial (as is the case for lambdas).

However, the following code would compile:

    Both b = new Both();
    b.Value = 10;

    Console.WriteLine(b.Value);

Because `Both` has only one definition for `Value`, and therefore is not ambiguous. We could reintroduce the ambiguity though; the following code snippet would compile, but the code snippet above won't:

    class Both : IBoth 
    {
      private int value;

      int IGet.Value { get { return value; } }
      int ISet.Value { set { this.value = value; } }
    }

The ambiguity can also be removed by cast to the appropriate interface. You can read more about this here (from the very reliable, and insightful Eric Lippert): <https://stackoverflow.com/a/20413958/1244630>
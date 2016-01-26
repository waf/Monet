# Monet
Option and Result types for .NET, [inspired by Rust](https://doc.rust-lang.org/book/error-handling.html). This README is a work in progress.

# The `Option<T>` type

**The quick explanation:**

Provides an `Option<T>` implementation with monadic LINQ support, pattern matching via a `.Match()` method, and implicit 
conversion from `T` to `Option.Some` and `null` to `Option.None`. Supports implicit and explicit Nullable conversion, 
along with C# 6 null conditional operator (`?.`) support.

**The longer and easier explanation with monkeys**:

In C#, we have `Nullable<T>` (or the short-hand syntax `T?`, like `int?` or `DateTime?`) for 
expressing the absence of a value type. It's great! By looking at the following method signature, 
we know that it's possible that the method may not return an integer index:
```csharp
int? FindIndexOfMonkey(Collection<Monkey> monkeys, Monkey monkeyOfInterest)
```
However, this only works for value types, like primitive types, structs, and enums. 
What about this signature?
```csharp
Monkey FindMonkeyAtIndex(int index)
```
What happens if we pass in a bad index? Maybe our returned Monkey is null, but we can't be sure. 
We either need to read the documentation (if it exists) or go read the source.

With the Option type, we could change the signature to this:
```csharp
Option<Monkey> FindMonkeyAtIndex(int index)
```
Now it's clear that we may not return a Monkey. 

An `Option<Monkey>` could in one of two states:

- `Option.None` - The absence of a monkey. Before, this would be represented by `null`.
- `Option.Some` - A monkey.

This is only half the story, though. It'd be a huge pain to check for `Option.Some` and `Option.None` 
everywhere in the code, and to unwrap our `Option<Monkey>.Some` to a plain ol' Monkey.

```csharp
Option<Monkey> charles = FindMonkeyByName("Charles");
// check if Charles is missing, unwrap from Option<Monkey>
Option<Tree> tree = GetTreeClosestTo(charles)
// check if tree is missing, unwrap from Option<Tree>
Option<Banana> banana = charles.PickBanana(tree)
// check if we could pick a banana, unwrap from Option<Banana>
charles.Eat(banana)
```

While this library does provide a `.Match` method that supports this "manual check" style of coding, there's a better way. If you think
of `Option<T>` as similar to `IEnumerable<T>`, we can just `Select` over it to do the intermediate transforms:

```csharp
var option = from charles in FindMonkeyByName("Charles")
              from tree in GetTreeClosestTo(charles)
              from banana in charles.PickBanana(tree)
              select new { charles, banana };

option.Match(
    Some: result => result.charles.Eat(result.banana),
    None: () => {});
```

More reading:

- [The Wikipedia page on Option types](https://en.wikipedia.org/wiki/Option_type).
- [Null References: The Billion Dollar Mistake](http://www.infoq.com/presentations/Null-References-The-Billion-Dollar-Mistake-Tony-Hoare).
- [Error handling in Rust](https://doc.rust-lang.org/book/error-handling.html) - A good tutorial for error-handling with Option and Result types.

# The `Result<T, TError>` type

The above Option type is great, but in some cases we want to return information about what went wrong, like error text or an exception.
In this case, we can use `Result<T, TError>`. Think of it like Option<T>, but the `None` case can hold an error object, `TError`.

- [Error handling in Rust](https://doc.rust-lang.org/book/error-handling.html) - A good tutorial for error-handling with Option and Result types.

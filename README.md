# About

* A no-GC version of C#'s `IEnumerator<T>` and LINQ
* Inspired by C++'s `<iterator>` and `<algorithm>` headers
* Useful for any C# project with additional support for Unity

# Getting Started

## Unity Projects

Clone or download this repository and copy the `JacksonDunstanIterator` directory somewhere inside your Unity project's `Assets` directory.

## Non-Unity Projects

Clone or download this repository and add all the `.cs` files directly under the `JacksonDunstanIterator` directory to your C# project.

# Usage

There are three iterator types:

* `ArrayIterator<T>` for managed arrays (`T[]`)
* `ListIterator<T>` for `List<T>`
* `NativeArrayIterator<T>` for `NativeArray<T>` (automatically compiled out when Unity 2018.1+ isn't available)

They all have the same API. Here's the basics of using `ArrayIterator<T>`:

```csharp
// Get an array
int[] array = new [] { 30, 10, 20, 40 };

// Get an iterator to the beginning of the array
ArrayIterator<int> begin = array.Begin();

// Get the value of the iterator
int val = begin.GetCurrent();

// Move to the next element
ArrayIterator<int> second = begin.GetNext();

// Get an iterator to one past the end of the array
ArrayIterator<int> end = array.End();

// Reverse [ 10, 20, 40] so the array is [ 30, 40, 20, 10 ]
second.Reverse(end);

// Search for an element satisfying a condition
ArrayIterator<int> it20 = begin.FindIf(end, e => e < 25);
```

There is much more functionality available. See [the source](JacksonDunstanIterator/ArrayIterator.cs) for more.

To read about the making of this library, see the _Enumerables Without the Garbage_ series:

* [Part 1](https://jacksondunstan.com/articles/3471)
* [Part 2](https://jacksondunstan.com/articles/3482)
* [Part 3](https://jacksondunstan.com/articles/3491)
* [Part 4](https://jacksondunstan.com/articles/3508)
* [Part 5](https://jacksondunstan.com/articles/3515)
* [Part 6](https://jacksondunstan.com/articles/3524)
* [Part 7](https://jacksondunstan.com/articles/3541)
* [Part 8](https://jacksondunstan.com/articles/4905)

# License

[MIT](LICENSE.txt)
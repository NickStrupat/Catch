# Catch
LINQ extention method for catching exceptions

```csharp
var items = new [] { "some", "strings", null, "and", "some" null, "nulls" };
var lengths = from items select item.Length;
var lengthsAndExceptions = lengths.Catch<NullReferenceException>()
                                  .Catch<ArgumentException>()
                                  .Rethrow<InvalidOperationException>();
var exceptions = lengthsAndExceptions.SelectExceptions();
var justLength = lengthsAndExceptions.SelectValues(); // `lengthsAndExceptions` only iterated once
```
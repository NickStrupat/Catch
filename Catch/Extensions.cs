using System;
using System.Collections.Generic;
using System.Linq;

namespace Tests {
    public static class Extensions
    {
        public static ExceptionCatchingEnumerable<TSource> Catch<TSource>(this IEnumerable<TSource> source) => new ExceptionCatchingEnumerable<TSource>(source);

        public static IEnumerable<TSource> NoExceptions<TSource>(this IEnumerable<Result<TSource, Exception>> sourceResults) => sourceResults.Where(x => x.Exception == null).Select(x => x.Value);
        public static IEnumerable<Exception> Exceptions<TSource>(this IEnumerable<Result<TSource, Exception>> sourceResults) => sourceResults.Where(x => x.Exception != null).Select(x => x.Exception);
    }
}
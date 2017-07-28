using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Xunit;

namespace Tests
{
    public class UnitTest1
    {
        [Fact]
        public void OnceEnumerableDirect()
        {
            var strings = new[] { "test", null, "the", "strings", null, null };
            var onceEnumerable = new OnceEnumerable<String>(strings);
            Assert.Equal(strings, onceEnumerable.ToArray());
            Assert.Throws<AlreadyOnceEnumeratedException>(() => onceEnumerable.ToArray());
        }

        [Fact]
        public void CatchAllExceptAlreadyOnceEnumeratedException()
        {
            var strings = new[] { "test", null, "the", "strings", null, null };
            var onceEnumerable = new OnceEnumerable<String>(strings);
            var lengthsAndCaught = onceEnumerable.Select(x => x.Length).Catch().ButNot<AlreadyOnceEnumeratedException>();
            Assert.Equal(strings.Select(x => x?.Length ?? 0), lengthsAndCaught.ToArray().Select(x => x.Value));
            Assert.Throws<AlreadyOnceEnumeratedException>(() => lengthsAndCaught.ToArray());
        }

        //[Fact]
        //public void CatchAll()
        //{
        //    var strings = new[] { "test", null, "the", "strings", null, null };
        //    var onceEnumerable = new OnceEnumerable<String>(strings);
        //    var lengthsAndCaught = onceEnumerable.Select(x => x.Length).CatchAll();
        //    Assert.Equal(strings.Select(x => x?.Length ?? 0), lengthsAndCaught.ToArray().Select(x => x.Value));
        //    Assert.Throws<AlreadyOnceEnumeratedException>(() => lengthsAndCaught.ToArray());
        //    return;
        //    var lengths2 = lengthsAndCaught.ToArray();
        //    var lengths3 = lengthsAndCaught.ToArray();
        //    var lengths = lengthsAndCaught.NoExceptions().ToArray();
        //    var caught = lengthsAndCaught.Exceptions().ToArray();
        //    //var lengthsAndCaught2 = strings.Select(x => x.Length).CatchAll().Thrown<NullReferenceException>();
        //    //var lengthsAndCaught2 = strings.Select(x => x.Length).CatchAll().Except<NullReferenceException>();
        //}
    }

    internal class AlreadyOnceEnumeratedException : InvalidOperationException { }

    internal class OnceEnumerable<TSource> : IEnumerable<TSource>
    {
        private readonly IEnumerable<TSource> source;
        public OnceEnumerable(IEnumerable<TSource> source) => this.source = source;

        //private Int32 enumerated;
        //public IEnumerator<TSource> GetEnumerator() => Interlocked.Exchange(ref enumerated, 1) == 1 ? throw new AlreadyOnceEnumeratedException() : source.GetEnumerator();
        //IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private Int32 enumerated;
        public IEnumerator<TSource> GetEnumerator()
        {
            return CreateEnumerable().GetEnumerator();

            IEnumerable<TSource> CreateEnumerable()
            {
                if (Interlocked.Exchange(ref enumerated, 1) == 1)
                    throw new AlreadyOnceEnumeratedException();
                foreach (var value in source)
                    yield return value;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
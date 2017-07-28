using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Tests {
    public sealed class ExceptionCatchingEnumerable<TSource> : IEnumerable<Result<TSource, Exception>>
    {
        private readonly IEnumerable<TSource> source;
        private readonly IEnumerable<IsEx> toCatch;
        private readonly IEnumerable<IsEx> toIgnore;

        internal ExceptionCatchingEnumerable(IEnumerable<TSource> source) => this.source = source;
        internal ExceptionCatchingEnumerable(IEnumerable<TSource> source, params IsEx[] toCatch) : this(source) => this.toCatch = toCatch ?? Enumerable.Empty<IsEx>();
        internal ExceptionCatchingEnumerable(IEnumerable<TSource> source, IsEx[] toCatch, params IsEx[] toIgnore) : this(source, toCatch) => this.toIgnore = toIgnore ?? Enumerable.Empty<IsEx>();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public IEnumerator<Result<TSource, Exception>> GetEnumerator()
        {
            return CreateEnumerable().GetEnumerator();

            IEnumerable<Result<TSource, Exception>> CreateEnumerable()
            {
                var result = new Result<TSource, Exception>();
                using (var enumerator = source.GetEnumerator())
                {
                    var next = false;
                    do
                    {
                        try
                        {
                            next = enumerator.MoveNext();
                            if (!next)
                                yield break;
                            result = new Result<TSource, Exception>(enumerator.Current);
                        }
                        catch (Exception ex) when (ShouldCatch(ex))
                        {
                            result = new Result<TSource, Exception>(ex);
                        }

                        yield return result;
                    } while (next);
                }
            }
        }

        private Boolean ShouldCatch(Exception ex) {
            var @catch = toCatch.Length == 0;
            for (var i = 0; i != toCatch.Length; i++)
                if (toCatch[i].Invoke(ex)) {
                    @catch = true;
                    break;
                }
            for (var i = 0; i != toIgnore.Length; i++)
                if (toIgnore[i].Invoke(ex)) {
                    @catch = false;
                    break;
                }
            return @catch;
        }

        internal delegate Boolean IsEx(Exception exception);
        private static Boolean Is<TException>(Exception exception) => exception is TException;

        public ExceptionCatchingEnumerable<TSource> Thrown<TException>()
            where TException : Exception
            => new ExceptionCatchingEnumerable<TSource>(source, toCatch.Concat(new IsEx[] { Is<TException> }).ToArray());



        public ExceptionCatchingEnumerable<TSource> ButNot<TException>() where TException : Exception => new ExceptionCatchingEnumerable<TSource>(source, toCatch, Is<TException>);
    }
}
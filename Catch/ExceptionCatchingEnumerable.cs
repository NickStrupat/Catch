using System;
using System.Collections;
using System.Collections.Generic;

namespace Tests {
    public sealed class ExceptionCatchingEnumerable<TSource> : IEnumerable<Result<TSource, Exception>>
    {
        private readonly IEnumerable<TSource> source;
        private readonly IsEx[] toCatch;
        private readonly IsEx[] toIgnore;

        internal ExceptionCatchingEnumerable(IEnumerable<TSource> source) => this.source = source;
        internal ExceptionCatchingEnumerable(IEnumerable<TSource> source, params IsEx[] toCatch) : this(source) => this.toCatch = toCatch ?? Array.Empty<IsEx>();
        internal ExceptionCatchingEnumerable(IEnumerable<TSource> source, IsEx[] toCatch, params IsEx[] toIgnore) : this(source, toCatch) => this.toIgnore = toIgnore ?? Array.Empty<IsEx>();

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

        public ExceptionCatchingEnumerable<TSource> Types<TException>()
            where TException : Exception
            => new ExceptionCatchingEnumerable<TSource>(source, Is<TException>);

        public ExceptionCatchingEnumerable<TSource> Types<TEx1, TEx2>()
            where TEx1 : Exception
            where TEx2 : Exception
            => new ExceptionCatchingEnumerable<TSource>(source, Is<TEx1>, Is<TEx2>);

        public ExceptionCatchingEnumerable<TSource> Types<TEx1, TEx2, TEx3>()
            where TEx1 : Exception
            where TEx2 : Exception
            where TEx3 : Exception
            => new ExceptionCatchingEnumerable<TSource>(source, Is<TEx1>, Is<TEx2>, Is<TEx3>);

        public ExceptionCatchingEnumerable<TSource> Types<TEx1, TEx2, TEx3, TEx4>()
            where TEx1 : Exception
            where TEx2 : Exception
            where TEx3 : Exception
            where TEx4 : Exception
            => new ExceptionCatchingEnumerable<TSource>(source, Is<TEx1>, Is<TEx2>, Is<TEx3>, Is<TEx4>);



        public ExceptionCatchingEnumerable<TSource> Types<TEx1, TEx2, TEx3, TEx4, TEx5, TEx6, TEx7, TEx8, TEx9>()
            where TEx1 : Exception
            where TEx2 : Exception
            where TEx3 : Exception
            where TEx4 : Exception
            where TEx5 : Exception
            where TEx6 : Exception
            where TEx7 : Exception
            where TEx8 : Exception
            where TEx9 : Exception
            => new ExceptionCatchingEnumerable<TSource>(source, Is<TEx1>, Is<TEx2>, Is<TEx3>, Is<TEx4>, Is<TEx5>, Is<TEx6>, Is<TEx7>, Is<TEx8>, Is<TEx9>);

        public ExceptionCatchingEnumerable<TSource> Types<TEx1, TEx2, TEx3, TEx4, TEx5, TEx6, TEx7, TEx8, TEx9, TEx10>()
            where TEx1 : Exception
            where TEx2 : Exception
            where TEx3 : Exception
            where TEx4 : Exception
            where TEx5 : Exception
            where TEx6 : Exception
            where TEx7 : Exception
            where TEx8 : Exception
            where TEx9 : Exception
            where TEx10 : Exception
            => new ExceptionCatchingEnumerable<TSource>(source, Is<TEx1>, Is<TEx2>, Is<TEx3>, Is<TEx4>, Is<TEx5>, Is<TEx6>, Is<TEx7>, Is<TEx8>, Is<TEx9>, Is<TEx10>);



        public ExceptionCatchingEnumerable<TSource> ButNot<TException>() where TException : Exception => new ExceptionCatchingEnumerable<TSource>(source, toCatch, Is<TException>);
    }
}
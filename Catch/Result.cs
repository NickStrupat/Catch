using System;

namespace Tests {
    public struct Result<T, TException> where TException : Exception
    {
        public readonly T Value;
        public readonly TException Exception;
        public Result(T value) : this() => Value = value;
        public Result(TException exception) : this() => Exception = exception;
    }
}
using System;
using System.Diagnostics.Contracts;

namespace Monet.Options
{
    public struct Option<T> 
    {
        readonly internal T Value;
        readonly internal bool HasValue;

        public Option(T value)
        {
            if(value == null)
            {
                Value = default(T);
                HasValue = false;
            }
            else
            {
                Value = value;
                HasValue = true;
            }
        }

        public readonly static Option<T> None = new Option<T>();
        public static Option<TValue> Some<TValue>(TValue value)
        {
            Contract.Requires<ArgumentNullException>(value != null);
            return new Option<TValue>(value);
        }

        public override string ToString()
        {
            return HasValue ? $"Option.Some<{typeof(T).Name}>: {Value.ToString()}"
                            : $"Option.None<{typeof(T).Name}>";
        }

        public T GetValueOrDefault(T defaultValue)
        {
            Contract.Requires<ArgumentNullException>(defaultValue != null);
            return HasValue ? Value : defaultValue;
        }

        public static implicit operator Option<T>(T t)
        {
            if(t == null)
            {
                return None;
            }
            return new Option<T>(t);
        }

        // allows non-generic Option.None to convert to generic Option<T>
        public static implicit operator Option<T>(OptionNone t)
        {
            return None;
        }
    }

    /// <summary>
    /// A non-generic representation of None, can implicitly convert to any Option<T>.None
    /// </summary>
    public struct OptionNone { };

    public static class Option
    {
        public readonly static OptionNone None = new OptionNone();
        public static Option<T> Some<T>(T value)
        {
            Contract.Requires<ArgumentNullException>(value != null);
            return new Option<T>(value);
        }

        public static Option<T> Try<T>(Func<T> func)
        {
            Contract.Requires<ArgumentNullException>(func != null);
            try
            {
                return func();
            }
            catch
            {
                return new Option<T>();
            }
        }
    }
}

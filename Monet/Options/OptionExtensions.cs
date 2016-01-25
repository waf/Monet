using Monet.Results;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Monet.Options
{
    public static class OptionExtensions
    {
        public static Option<TResult> Select<TSource, TResult>(
            this Option<TSource> source,
            Func<TSource, TResult> selector)
        {
            Contract.Requires<ArgumentNullException>(selector != null);
            if (!source.HasValue)
                return Option<TResult>.None;
            return new Option<TResult>(selector(source.Value));
        }

        public static Option<T> ToOption<T>(this Nullable<T> nullable) where T : struct
        {
            return nullable.HasValue ? Option.Some(nullable.Value) : Option.None;
        }

        public static T? ToNullable<T>(this Option<T> option) where T : struct
        {
            return option.HasValue ? new T?(option.Value) : null;
        }

        public static Result<T, TError> ToResult<T, TError>(this Option<T> option, TError error)
        {
            Contract.Requires<ArgumentNullException>(error != null);
            return option.HasValue
                ? Result<T, TError>.Ok(option.Value)
                : Result<T, TError>.Error(error);
        }

        public static TOutput Match<T, TOutput>(
            this Option<T> option,
            Func<T, TOutput> Some = null, Func<TOutput> None = null)
        {
            Contract.Requires<ArgumentNullException>(Some != null);
            Contract.Requires<ArgumentNullException>(None != null);

            return option.HasValue ? Some(option.Value) : None();
        }

        public static void Match<T>(
            this Option<T> option,
            Action<T> Some = null, Action None = null)
        {
            Contract.Requires<ArgumentNullException>(Some != null);
            Contract.Requires<ArgumentNullException>(None != null);

            if (option.HasValue)
                Some(option.Value);
            else
                None();
        }
    }
}

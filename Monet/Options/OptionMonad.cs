using System;
using System.Diagnostics.Contracts;

namespace Monet.Options
{
    public static class OptionMonad
    {
        // Bind
        public static Option<TResult> SelectMany<TSource, TResult>(
            this Option<TSource> self,
            Func<TSource, Option<TResult>> projector)
        {
            Contract.Requires<ArgumentNullException>(projector != null);

            if (!self.HasValue)
                return Option<TResult>.None;
            return projector(self.Value);
        }

        /// <summary>
        /// The following code:
        ///   from m0 in m
        ///   from n0 in n
        ///   select m0 + n0
        /// would be translated in:
        ///   m.SelectMany(m0 => n, (m, n0) => m0 + n0);
        /// we want to make it instead invoke SelectMany twice
        /// </summary>
        public static Option<TResult> SelectMany<TSource, TOption, TResult>(
            this Option<TSource> m,
            Func<TSource, Option<TOption>> projector,
            Func<TSource, TOption, TResult> selector)
        {
            Contract.Requires<ArgumentNullException>(projector != null);
            Contract.Requires<ArgumentNullException>(selector != null);

            return m.SelectMany(t => projector(t)
                .SelectMany(u => new Option<TResult>(selector(t, u))));
        }
    }
}

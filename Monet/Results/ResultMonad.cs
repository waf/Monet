using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monet.Results
{
    public static class ResultMonad
    {
        // Bind
        public static Result<TOkOutput, TError> SelectMany<TOkInput, TError, TOkOutput>(
            this Result<TOkInput, TError> self,
            Func<TOkInput, Result<TOkOutput,TError>> selector)
        {
            Contract.Requires<ArgumentNullException>(selector != null);

            if (!self.isOk)
                return Result<TOkOutput, TError>.Error(self.error);
            return selector(self.ok);
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
        public static Result<TOutput,TError> SelectMany<TInput, TError, TIntermediate, TOutput>(
            this Result<TInput, TError> self,
            Func<TInput, Result<TIntermediate,TError>> selector,
            Func<TInput, TIntermediate, TOutput> projector)
        {
            Contract.Requires<ArgumentNullException>(selector != null);
            Contract.Requires<ArgumentNullException>(projector != null);

            return self.SelectMany(r =>
                selector(r).SelectMany(u => Result<TOutput, TError>.Ok(projector(self.ok, u))));
        }
    }
}

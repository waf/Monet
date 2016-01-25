using System;
using System.Diagnostics.Contracts;

namespace Monet.Results
{
    public static class ResultExtensions
    {
        public static Result<TOkOutput, TError> Select<TOkInput, TOkOutput, TError>(
            this Result<TOkInput, TError> self,
            Func<TOkInput, TOkOutput> selector)
        {
            Contract.Requires<ArgumentNullException>(selector != null);
            if (!self.isOk)
                return Result<TOkOutput, TError>.Error(self.error);
            return Result<TOkOutput, TError>.Ok(selector(self.ok));
        }

        public static Result<TOk, TErrorOutput> SelectError<TOk, TErrorInput, TErrorOutput>(
            this Result<TOk, TErrorInput> self,
            Func<TErrorInput, TErrorOutput> selector)
        {
            Contract.Requires<ArgumentNullException>(selector != null);
            if (self.isOk)
                return Result<TOk, TErrorOutput>.Ok(self.ok);
            return Result<TOk, TErrorOutput>.Error(selector(self.error));
        }

        public static Result<TOk, Exception> SelectAsException<TOk, TError>(
            this Result<TOk, TError> self) where TError : Exception
        {
            return self.SelectError(ex => ex as Exception);
        }

        public static TOutput Match<TOk, TError, TOutput>(
            this Result<TOk, TError> self,
            Func<TOk, TOutput> Ok = null, Func<TError, TOutput> Error = null)
        {
            Contract.Requires<ArgumentNullException>(Ok != null);
            Contract.Requires<ArgumentNullException>(Error != null);

            return self.isOk ? Ok(self.ok) : Error(self.error);
        }

        public static void Match<TOk, TError>(
            this Result<TOk, TError> self,
            Action<TOk> Ok = null, Action<TError> Error = null)
        {
            Contract.Requires<ArgumentNullException>(Ok != null);
            Contract.Requires<ArgumentNullException>(Error != null);

            if (self.isOk)
                Ok(self.ok);
            else
                Error(self.error);
        }
    }
}

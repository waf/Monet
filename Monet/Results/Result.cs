using System;
using System.Diagnostics.Contracts;

namespace Monet.Results
{
    public struct Result<TOk, TError> 
    {
        readonly internal TOk ok;
        readonly internal bool isOk;
        readonly internal TError error;

        private Result(bool isOk, TOk ok = default(TOk), TError error = default(TError))
        {
            this.isOk = isOk;
            if (isOk)
            {
                this.ok = ok;
                this.error = default(TError);
            }
            else
            {
                this.ok = default(TOk);
                this.error = error;
            }
        }

        public static Result<TOk, TError> Ok(TOk value)
        {
            Contract.Requires<ArgumentNullException>(value != null);
            return new Result<TOk, TError>(true, ok: value);
        }

        public static Result<TOk, TError> Error(TError error)
        {
            Contract.Requires<ArgumentNullException>(error != null);
            return new Result<TOk, TError>(false, error: error);
        }

        public override string ToString()
        {
            return isOk ? $"Result.Ok<{typeof(TOk).Name}>: {ok.ToString()}"
                        : $"Result.Error<{typeof(TError).Name}>: {error.ToString()}";
        }

        public static implicit operator Result<TOk, TError>(TOk value)
        {
            Contract.Requires<ArgumentNullException>(value != null);
            return new Result<TOk, TError>(true, ok: value);
        }

        public static implicit operator Result<TOk, TError>(TError error)
        {
            Contract.Requires<ArgumentNullException>(error != null);
            return new Result<TOk, TError>(false, error: error);
        }
    }

    public static class Result
    {
        public static Result<TOk, Exception> Try<TOk>(Func<TOk> func)
        {
            Contract.Requires<ArgumentNullException>(func != null);
            try
            {
                return Result<TOk, Exception>.Ok(func());
            }
            catch (Exception e)
            {
                return Result<TOk, Exception>.Error(e);
            }
        }
    }
}

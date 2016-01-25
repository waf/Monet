using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using Monet.Results;
using Monet.Options;

namespace Monet.Tests
{
    [TestClass]
    public class ResultTests
    {
        [TestMethod]
        public void TestMonadicSyntax_Success()
        {
            Result<int, string> b = 5;
            Result<int, string> a = 6;

            var result = from aval in a
                         from bval in b
                         select aval + bval;

            Assert.AreEqual(11, result);

        }

        [TestMethod]
        public void TestMonadicSyntax_Failure()
        {
            const string error = "uhoh i'm an error";
            Result<int, string> b = error;
            Result<int, string> a = 6;

            var result = from aval in a
                         from bval in b
                         select aval + bval;

            var result2 = from bval in b
                          from aval in a
                          select bval + aval;

            Assert.AreEqual(error, result);
            Assert.AreEqual(error, result2);

        }

        [TestMethod]
        public void TestDoubleArgument()
        {
            Func<string[], int, Option<string>> nth = (arr, n) => Option.Try(() => arr[n]);
            Func<string, Result<int, Exception>> parseInt = str => Result.Try(() => int.Parse(str));
            string[] args = { "5" };

            var result = nth(args, 0)
                .ToResult("Please give at least 1 element")
                .SelectMany(element => parseInt(element).SelectError(ex => ex.Message))
                .Select(n => n * 2);

            var output = result.Match(
                Ok: ok => $"Your answer is {ok}",
                Error: error => $"Sorry, {error}");

            Assert.AreEqual("Your answer is 10", output);

            args = new string[0];
            result = nth(args, 0)
                .ToResult("Please give at least 1 element.")
                .SelectMany(element => parseInt(element).SelectError(ex => ex.Message))
                .Select(n => n * 2);

            output = result.Match(
                Ok: ok => $"Your answer is {ok}",
                Error: error => $"Sorry, {error}");

            Assert.AreEqual("Sorry, Please give at least 1 element.", output);

            args = new[] { "i'm not an integer" };
            result = nth(args, 0)
                .ToResult("Please give at least 1 element")
                .SelectMany(element => parseInt(element).SelectError(ex => ex.Message))
                .Select(n => n * 2);

            output = result.Match(
                Ok: ok => $"Your answer is {ok}",
                Error: error => $"Sorry, {error}");

            Assert.AreEqual("Sorry, Input string was not in a correct format.", output);
        }

        [TestMethod]
        public void ExceptionResults()
        {
            Func<Result<int, InvalidOperationException>> FailsWithExceptionA = () => new InvalidOperationException();
            Func<Result<int, ArgumentException>> FailsWithExceptionB = () => new ArgumentException();

            var a = from i in FailsWithExceptionA().SelectAsException()
                    from j in FailsWithExceptionB().SelectAsException()
                    select i + j;

            a.Match(
                Ok: i => Assert.Fail("should not be ok"),
                Error: (msg) => Assert.IsNotNull(msg));
        }
    }
}

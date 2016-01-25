using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Monet;
using Monet.Options;

namespace Monet.Tests
{
    [TestClass]
    public class OptionTests
    {
        [TestMethod]
        public void MonadicSyntaxSuccess()
        {
            Option<int> o1 = 6;
            Option<int> o2 = 7;
            var success = from a in o1
                          from b in o2
                          select a + b;

            Assert.AreEqual(13, success);
            Assert.AreEqual(Option.Some(13), success);

            //make sure that option<int> 0 isn't considered none
            o1 = 0;
            o2 = 1;
            success = from a in o1
                      from b in o2
                      select a + b;
            Assert.AreEqual(1, success);
            Assert.AreEqual(Option.Some(1), success);

            // nullable 0 shouldn't be none
            Option<int?> zeroMaybe = 0;
            Option<int?> oneMaybe = 1;
            Option<int?> successMaybe = from a in zeroMaybe
                                        from b in oneMaybe
                                        select a + b;
            Assert.AreEqual(1, successMaybe);
            Assert.AreEqual(Option.Some(new int?(1)), successMaybe);

            // a "null" nullable should be none
            zeroMaybe = 0;
            oneMaybe = null;
            successMaybe = from a in zeroMaybe
                           from b in oneMaybe
                           select a + b;
            Assert.AreEqual(null, successMaybe);
            Assert.AreEqual(Option.None, successMaybe);
            Assert.AreNotEqual(0, successMaybe);
        }
        [TestMethod]
        public void MonadicSyntaxFailure()
        {
            Option<int> o1 = 6;
            Option<int> o2 = Option.None;
            Option<int> o3 = 7;
            var failure = from a in o1
                          from b in o2
                          from c in o3
                          select a + b + c;

            Assert.AreEqual(Option.None, failure);
        }

        [TestMethod]
        public void TestNullImplicitlyNone()
        {
            Func<Option<string>> ReturnsNone = () => null;

            Option<string> result = ReturnsNone();
            Assert.AreEqual(Option.None, result);
        }

        [TestMethod]
        public void TestInitialization()
        {
            Option<string> o1 = Option.None;
            Assert.AreEqual(Option.None, o1);

            Option<string> o2 = Option<string>.None;
            Assert.AreEqual(Option.None, o2);

            Option<string> o3 = null;
            Assert.AreEqual(Option.None, o3);

            Option<string> o4 = Option.Some("abc");
            Assert.AreEqual("abc", o4);

            Option<string> o5 = Option<string>.Some("abc");
            Assert.AreEqual("abc", o5);
            Assert.AreEqual(Option.Some("abc"), o5);

            Option<string> o6 = new Option<string>("abc");
            Assert.AreEqual("abc", o6);
            Assert.AreEqual(Option.Some("abc"), o6);

            Option<string> o7 = "abc";
            Assert.AreEqual("abc", o7);
            Assert.AreEqual(Option.Some("abc"), o7);

            Option<int> o8 = 0;
            Assert.AreEqual(Option.Some(0), o8);
        }

        [TestMethod]
        public void TestSelect_OnNone()
        {
            Option<string> noneOption = Option.None;

            Option<string> result = noneOption.Select(a => a + "d");
            Assert.AreEqual(Option.None, result);
        }

        [TestMethod]
        public void TestSelect_OnSome()
        {
            string input = "abc";
            Option<string> someOption = Option.Some(input);

            Option<string> result = someOption.Select(a => a + "d");
            Assert.AreEqual(Option.Some("abcd"), result);
            Assert.AreEqual("abcd", result);
        }

        [TestMethod]
        public void TestSelectMany_OnSome()
        {
            Func<string, Option<int>> ParseToInt = str =>
            {
                try { return Option.Some(int.Parse(str)); }
                catch { return Option.None; }
            };
            Func<int, int, Option<int>> IntegerDivison = (dividend, divisor) =>
            {
                return divisor == 0 ? Option.None : Option.Some(dividend / divisor);
            };

            Option<int> someSelectMany = ParseToInt("6").SelectMany(num => IntegerDivison(num, 2));
            Option<int> someLinq = from parsed in ParseToInt("6")
                                   from divided in IntegerDivison(parsed, 2)
                                   select divided;
            Assert.AreEqual(Option.Some(3), someSelectMany);
            Assert.AreEqual(Option.Some(3), someLinq);

            Option<int> parseFailSelectMany = ParseToInt("q").SelectMany(num => IntegerDivison(num, 2));
            Option<int> parseFailLinq = from parsed in ParseToInt("q")
                                        from divided in IntegerDivison(parsed, 2)
                                        select divided;
            Assert.AreEqual(Option.None, parseFailSelectMany);
            Assert.AreEqual(Option.None, parseFailLinq);

            Option<int> divideFailSelectMany = ParseToInt("6").SelectMany(num => IntegerDivison(num, 0));
            Option<int> divideFailLinq = from parsed in ParseToInt("6")
                                         from divided in IntegerDivison(parsed, 0)
                                         select divided;
            Assert.AreEqual(Option.None, divideFailSelectMany);
            Assert.AreEqual(Option.None, divideFailLinq);
        }


        [TestMethod]
        public void TestNullConditional()
        {
            var obj = new[]
            {
                new { A = new { number = 2, str = "foo" }  },
                null
            };

            Option<int?> numberResult = obj?[0]?.A?.number;
            Assert.AreEqual(2, numberResult);

            numberResult = obj?[1]?.A?.number;
            Assert.AreEqual(Option.None, numberResult);

            Option<string> strResult = obj?[0]?.A?.str;
            Assert.AreEqual("foo", strResult);

            strResult = obj?[1]?.A?.str;
            Assert.AreEqual(Option.None, strResult);
        }

        [TestMethod]
        public void TestNullables()
        {
            int? a = 5;
            var o = Option.Some(a);
            Assert.AreEqual(5, a);
            Assert.AreEqual(a, o);
            Assert.AreEqual(5, o);

            try
            {
                int? b = null;
                var o2 = Option.Some(b);
                Assert.Fail("Option.Some should throw exception when passed null");
            }
            catch
            {
                // pass scenario
            }

        }

    }
}

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tuto.Publishing.Youtube.Tests
{
    [TestClass]
    public class MatchTest
    {

        void Test(string s1, string s2, int match)
        {
            Assert.AreEqual(match, Algorithms.MatchNames(s1, s2));
            Assert.AreEqual(match, Algorithms.MatchNames(s2, s1));
        }

        [TestMethod]
        public void Simple() { Test("abc", "abc", 3); }

        [TestMethod]
        public void Insert() { Test("abc", "abxyc", 3); }
        
        [TestMethod]
        public void Various() { Test("xyazubxcz", "fafgbfdch", 3); }

        [TestMethod]
        public void Strings() { Test("abc", "fffabc", 3); }


    }
}

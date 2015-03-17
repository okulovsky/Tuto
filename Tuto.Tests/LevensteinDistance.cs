using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tuto.Publishing.Tests
{
    [TestClass]
    public class LevensteinTest
    {

        void Test(string s1, string s2, int match)
        {
			Assert.AreEqual(match, Tuto.Publishing.Matching.LevensteinDistance.MatchNames(s1, s2));
			Assert.AreEqual(match, Tuto.Publishing.Matching.LevensteinDistance.MatchNames(s2, s1));
        }

        [TestMethod]
        public void Simple() { Test("abc", "abc", 0); }

        [TestMethod]
        public void Insert() { Test("abc", "abxyc", 2); }
        
        [TestMethod]
        public void Various() { Test("xyazubxcz", "fafgbfdch", 12); }

        [TestMethod]
        public void Strings() { Test("abc", "fffabc", 3); }


    }
}

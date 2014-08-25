using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tuto.Model;

namespace Tuto.Tests
{
    [TestClass]
    public class SrtMakerTests
    {
        [TestMethod]
        public void TestMsOutput()
        {
            Assert.AreEqual("00:00:10,500", SrtMaker.MsInSrtFormat(10500));
            Assert.AreEqual("00:02:10,500", SrtMaker.MsInSrtFormat((2 * 60 + 10) * 1000 + 500));
        }


        [TestMethod]
        public void TestLocation()
        {
            var array = new StreamChunk[]
            {
                new StreamChunk(0000,1000, Editor.Mode.Drop, false),
                new StreamChunk(1000,2000, Editor.Mode.Face, false),
                new StreamChunk(2000,3000, Editor.Mode.Drop, false),
                new StreamChunk(3000,4000, Editor.Mode.Screen, false),
            };
            Assert.AreEqual(1500, SrtMaker.FindFinalLocation(3500, array));
        }
    }
}

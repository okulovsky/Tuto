using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tuto;
using Tuto.TutoServices.Assembler;
using Tuto.TutoServices;
using Tuto.TutoServices.Montager;
using Tuto.Model;
using Editor;
using System.IO;

namespace VideoMontageTester
{
    [TestClass]
    public class UnitTest1
    {
        private AssemblerService serv;
        private EditorModel model;

        [TestMethod]
        public void SimpleDesktopCutTest()
        {
            SetUp();
            model.Montage.Chunks.Mark(1000, new bool[] { false, true }, false);
            model.FormPreparedChunks();
            Assert.AreEqual(model.Montage.PreparedChunks.Count, 2); //plus dropped tail
            Assert.AreEqual(model.Montage.PreparedChunks[0].Mode, Mode.Desktop);
            Assert.AreEqual(model.Montage.PreparedChunks[0].StartTime, 0);
            Assert.AreEqual(model.Montage.PreparedChunks[0].EndTime, 1000);
        }

        [TestMethod]
        public void SimpleFaceCutTest()
        {
            SetUp();
            model.Montage.Chunks.Mark(1000, new bool[] { true, true }, false);
            model.FormPreparedChunks();
            Assert.AreEqual(model.Montage.PreparedChunks.Count, 2); //plus dropped tail
            Assert.AreEqual(model.Montage.PreparedChunks[0].Mode, Mode.Face);
            Assert.AreEqual(model.Montage.PreparedChunks[0].StartTime, 0);
            Assert.AreEqual(model.Montage.PreparedChunks[0].EndTime, 1000);
        }

        
        [TestMethod]
        public void CollapseVideoChunkTest()
        {
            SetUp();
            model.Montage.Chunks.Mark(1000, new bool[] {true, true}, false);
            model.Montage.Chunks.Mark(2000, new bool[] { true, true }, false);
            model.FormPreparedChunks();
            Assert.AreEqual(model.Montage.PreparedChunks.Count, 2); //plus dropped tail
            Assert.AreEqual(model.Montage.PreparedChunks[0].StartTime, 0);
            Assert.AreEqual(model.Montage.PreparedChunks[0].EndTime, 2000);
            Assert.AreEqual(model.Montage.PreparedChunks[0].Mode, Mode.Face);
        }

        [TestMethod]
        public void CollapseDesktopChunkTest()
        {
            SetUp();
            var serv = new AssemblerService();
            var model = new EditorModel(new DirectoryInfo("test"), new DirectoryInfo("test"), new DirectoryInfo("test"));
            model.Montage.Chunks.Mark(1000, new bool[] { false, true }, false);
            model.Montage.Chunks.Mark(2000, new bool[] { false, true }, false);
            model.FormPreparedChunks();
            Assert.AreEqual(model.Montage.PreparedChunks.Count, 2); //plus dropped tail
            Assert.AreEqual(model.Montage.PreparedChunks[0].StartTime, 0);
            Assert.AreEqual(model.Montage.PreparedChunks[0].EndTime, 2000);
            Assert.AreEqual(model.Montage.PreparedChunks[0].Mode, Mode.Desktop);
        }

        [TestMethod]
        public void SkipInsideVideoTest()
        {
            SetUp();
            model.Montage.Chunks.Mark(1000, new bool[] { true, true }, false);
            model.Montage.Chunks.Mark(2000, new bool[] { false, false }, false);
            model.Montage.Chunks.Mark(3000, new bool[] { true, true }, false);
            model.FormPreparedChunks();
            Assert.AreEqual(model.Montage.PreparedChunks.Count, 4); //plus dropped tail and middle
            Assert.AreEqual(model.Montage.PreparedChunks[0].StartTime, 0);
            Assert.AreEqual(model.Montage.PreparedChunks[0].EndTime, 1000);
            Assert.AreEqual(model.Montage.PreparedChunks[2].StartTime, 2000);
            Assert.AreEqual(model.Montage.PreparedChunks[2].EndTime, 3000);
        }

        [TestMethod]
        public void NewEpisodeTest()
        {
            SetUp();
            model.Montage.Chunks.Mark(1000, new bool[] { true, true }, false);
            model.Montage.Chunks.NewEpisode(0);
            model.FormPreparedChunks();
            Assert.AreEqual(model.Montage.PreparedChunks.Count, 2); //plus dropped tail
            Assert.AreEqual(model.Montage.PreparedChunks[0].StartsNewEpisode, true);
        }


        [TestMethod]
        public void NotNewEpisodeTest()
        {
            SetUp();
            model.Montage.Chunks.Mark(1000, new bool[] { true, true }, false);
            model.Montage.Chunks.Mark(2000, new bool[] { false, true }, false);
            model.Montage.Chunks.NewEpisode(1);
            model.FormPreparedChunks();
            Assert.AreEqual(model.Montage.PreparedChunks.Count, 3); //plus dropped tail
            Assert.AreEqual(model.Montage.PreparedChunks[0].StartsNewEpisode, false);
        }

        [TestMethod]
        public void MixDesktopFace()
        {
            SetUp();
            model.Montage.Chunks.Mark(1000, new bool[] { true, true }, false);
            model.Montage.Chunks.Mark(2000, new bool[] { false, true }, false);
            model.FormPreparedChunks();
            Assert.AreEqual(model.Montage.PreparedChunks.Count, 3); //plus dropped tail
            Assert.AreEqual(model.Montage.PreparedChunks[0].Mode, Mode.Face);
            Assert.AreEqual(model.Montage.PreparedChunks[1].Mode, Mode.Desktop);
        }

        private void SetUp()
        {
            serv = new AssemblerService();
            model = new EditorModel(new DirectoryInfo("test"), new DirectoryInfo("test"), new DirectoryInfo("test"));
        }
    }
}

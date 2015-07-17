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
    public class MontageTests
    {
        private AssemblerService serv;
        private EditorModel model;

        [TestMethod]
        public void SimpleDesktopCutTest()
        {
            SetUp();
            model.Montage.Chunks.Mark(1000, new bool[] { false, true }, false);
            model.FormPreparedChunks();
            Assert.AreEqual(2, model.Montage.PreparedChunks.Count); //plus dropped tail
            Assert.AreEqual(Mode.Desktop, model.Montage.PreparedChunks[0].Mode);
            Assert.AreEqual(0, model.Montage.PreparedChunks[0].StartTime);
            Assert.AreEqual(1000, model.Montage.PreparedChunks[0].EndTime);
        }

        [TestMethod]
        public void SimpleFaceCutTest()
        {
            SetUp();
            model.Montage.Chunks.Mark(1000, new bool[] { true, true }, false);
            model.FormPreparedChunks();
            Assert.AreEqual(2, model.Montage.PreparedChunks.Count, 2); //plus dropped tail
            Assert.AreEqual(Mode.Face, model.Montage.PreparedChunks[0].Mode);
            Assert.AreEqual(0, model.Montage.PreparedChunks[0].StartTime);
            Assert.AreEqual(1000, model.Montage.PreparedChunks[0].EndTime);
        }

        
        [TestMethod]
        public void CollapseVideoChunkTest()
        {
            SetUp();
            model.Montage.Chunks.Mark(1000, new bool[] {true, true}, false);
            model.Montage.Chunks.Mark(2000, new bool[] { true, true }, false);
            model.FormPreparedChunks();
            Assert.AreEqual(2, model.Montage.PreparedChunks.Count); //plus dropped tail
            Assert.AreEqual(0, model.Montage.PreparedChunks[0].StartTime);
            Assert.AreEqual(2000, model.Montage.PreparedChunks[0].EndTime);
            Assert.AreEqual(Mode.Face, model.Montage.PreparedChunks[0].Mode);
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
            Assert.AreEqual(2, model.Montage.PreparedChunks.Count); //plus dropped tail
            Assert.AreEqual(0, model.Montage.PreparedChunks[0].StartTime);
            Assert.AreEqual(2000, model.Montage.PreparedChunks[0].EndTime);
            Assert.AreEqual(Mode.Desktop, model.Montage.PreparedChunks[0].Mode);
        }

        [TestMethod]
        public void SkipInsideVideoTest()
        {
            SetUp();
            model.Montage.Chunks.Mark(1000, new bool[] { true, true }, false);
            model.Montage.Chunks.Mark(2000, new bool[] { false, false }, false);
            model.Montage.Chunks.Mark(3000, new bool[] { true, true }, false);
            model.FormPreparedChunks();
            Assert.AreEqual(4,model.Montage.PreparedChunks.Count); //plus dropped tail and middle
            Assert.AreEqual(0, model.Montage.PreparedChunks[0].StartTime, 0);
            Assert.AreEqual(1000, model.Montage.PreparedChunks[0].EndTime, 1000);
            Assert.AreEqual(2000, model.Montage.PreparedChunks[2].StartTime, 2000);
            Assert.AreEqual(3000, model.Montage.PreparedChunks[2].EndTime, 3000);
        }

        [TestMethod]
        public void NewEpisodeTest()
        {
            SetUp();
            model.Montage.Chunks.Mark(1000, new bool[] { true, true }, false);
            model.Montage.Chunks.NewEpisode(0);
            model.FormPreparedChunks();
            Assert.AreEqual(2, model.Montage.PreparedChunks.Count); //plus dropped tail
            Assert.AreEqual(true, model.Montage.PreparedChunks[0].StartsNewEpisode);
        }


        [TestMethod]
        public void NotNewEpisodeTest()
        {
            SetUp();
            model.Montage.Chunks.Mark(1000, new bool[] { true, true }, false);
            model.Montage.Chunks.Mark(2000, new bool[] { false, true }, false);
            model.Montage.Chunks.NewEpisode(1);
            model.FormPreparedChunks();
            Assert.AreEqual(3, model.Montage.PreparedChunks.Count); //plus dropped tail
            Assert.AreEqual(false, model.Montage.PreparedChunks[0].StartsNewEpisode);
        }

        [TestMethod]
        public void MixDesktopFace()
        {
            SetUp();
            model.Montage.Chunks.Mark(1000, new bool[] { true, true }, false);
            model.Montage.Chunks.Mark(2000, new bool[] { false, true }, false);
            model.FormPreparedChunks();
            Assert.AreEqual(3, model.Montage.PreparedChunks.Count, 3); //plus dropped tail
            Assert.AreEqual(Mode.Face, model.Montage.PreparedChunks[0].Mode);
            Assert.AreEqual(Mode.Desktop, model.Montage.PreparedChunks[1].Mode);
        }

        [TestMethod]
        public void CheckAVSFadeout()
        {
            SetUp();
            model.Montage.Chunks.Mark(1000, new bool[] { true, true }, false);
            model.Montage.Chunks.Mark(2000, new bool[] { false, true }, false);
            model.FormPreparedChunks();
            var nodes = serv.GetEpisodesNodes(model)[0] as AvsConcatList;
            Assert.AreEqual(1, nodes.Items.Count);
            Assert.AreEqual(typeof(AvsFadeOut), nodes.Items[0].GetType());
        }

        [TestMethod]
        public void CheckAVSCrossFade()
        {
            SetUp();
            model.Montage.Chunks.Mark(1000, new bool[] { true, true }, false);
            model.Montage.Chunks.Mark(2000, new bool[] { false, false }, false);
            model.Montage.Chunks.Mark(3000, new bool[] { true, true }, false);
            model.FormPreparedChunks();
            var nodes = serv.GetEpisodesNodes(model)[0] as AvsConcatList;
            Assert.AreEqual(typeof(AvsCrossFade), nodes.Items[0].ChildNodes[0].GetType());
        }

        [TestMethod]
        public void CheckAVSMix()
        {
            SetUp();
            model.Montage.Chunks.Mark(1000, new bool[] { true, true }, false);
            model.Montage.Chunks.Mark(2000, new bool[] { false, true }, false);
            model.Montage.Chunks.Mark(3000, new bool[] { true, true }, false);
            model.FormPreparedChunks();
            var nodes = serv.GetEpisodesNodes(model)[0] as AvsConcatList;
            Assert.AreEqual(typeof(AvsMix), nodes.Items[0].ChildNodes[0].GetType());
        }

        [TestMethod]
        public void CheckAVS_Mix_X_F() //long pattern after AVS is Mix_Face mix chunk then drop then face
        {
            SetUp();
            model.Montage.Chunks.Mark(1000, new bool[] { true, true }, false);
            model.Montage.Chunks.Mark(2000, new bool[] { false, true }, false);
            model.Montage.Chunks.Mark(3000, new bool[] { true, true }, false);
            model.Montage.Chunks.Mark(4000, new bool[] { false, false }, false);
            model.Montage.Chunks.Mark(5000, new bool[] { true, true }, false);
            model.FormPreparedChunks();
            var nodes = serv.GetEpisodesNodes(model)[0] as AvsConcatList;
            Assert.AreEqual(typeof(AvsMix), nodes.Items[0].ChildNodes[0].ChildNodes[0].GetType());
            Assert.AreEqual(typeof(AvsChunk), nodes.Items[0].ChildNodes[0].ChildNodes[1].GetType());
        }

        [TestMethod]
        public void CheckAVS_F_X_Mix()
        {
            SetUp();
            model.Montage.Chunks.Mark(1000, new bool[] { true, true }, false);
            model.Montage.Chunks.Mark(2000, new bool[] { false, false }, false);
            model.Montage.Chunks.Mark(3000, new bool[] { true, true }, false);
            model.Montage.Chunks.Mark(4000, new bool[] { false, true }, false);
            model.Montage.Chunks.Mark(5000, new bool[] { true, true }, false);
            model.FormPreparedChunks();
            var nodes = serv.GetEpisodesNodes(model)[0] as AvsConcatList;
            Assert.AreEqual(typeof(AvsChunk), nodes.Items[0].ChildNodes[0].ChildNodes[0].GetType());
            Assert.AreEqual(typeof(AvsMix), nodes.Items[0].ChildNodes[0].ChildNodes[1].GetType());
        }

        [TestMethod]
        public void CheckAVS_F_X_FMix() //Fmix - mix started with face. Dmix - desktop etc.
        {
            SetUp();
            model.Montage.Chunks.Mark(1000, new bool[] { true, true }, false);
            model.Montage.Chunks.Mark(2000, new bool[] { false, false }, false);
            model.Montage.Chunks.Mark(3000, new bool[] { true, true }, false);
            model.Montage.Chunks.Mark(4000, new bool[] { false, true }, false);
            model.Montage.Chunks.Mark(5000, new bool[] { true, true }, false);
            model.FormPreparedChunks();
            var nodes = serv.GetEpisodesNodes(model)[0] as AvsConcatList;
            Assert.AreEqual(typeof(AvsCrossFade), nodes.Items[0].ChildNodes[0].GetType());
        }

        [TestMethod]
        public void CheckAVS_MixF_X_F() //Fmix - mix started with face. Dmix - desktop etc.
        {
            SetUp();
            model.Montage.Chunks.Mark(1000, new bool[] { true, true }, false);
            model.Montage.Chunks.Mark(2000, new bool[] { false, true }, false);
            model.Montage.Chunks.Mark(3000, new bool[] { true, true }, false);

            model.Montage.Chunks.Mark(4000, new bool[] { false, false }, false);

            model.Montage.Chunks.Mark(5000, new bool[] { true, true }, false);
            model.FormPreparedChunks();
            var nodes = serv.GetEpisodesNodes(model)[0] as AvsConcatList;
            Assert.AreEqual(typeof(AvsCrossFade), nodes.Items[0].ChildNodes[0].GetType());
        }

        [TestMethod]
        public void CheckAVS_MixF_X_D() //check F_D transition. It should be without crossfades
        {
            SetUp();
            model.Montage.Chunks.Mark(1000, new bool[] { true, true }, false);
            model.Montage.Chunks.Mark(2000, new bool[] { false, true }, false);
            model.Montage.Chunks.Mark(3000, new bool[] { true, true }, false);

            model.Montage.Chunks.Mark(4000, new bool[] { false, false }, false);

            model.Montage.Chunks.Mark(5000, new bool[] { false, true }, false);
            model.FormPreparedChunks();
            var nodes = serv.GetEpisodesNodes(model)[0] as AvsConcatList;
            Assert.AreNotEqual(typeof(AvsCrossFade), nodes.Items[0].ChildNodes[0].GetType());
        }

        [TestMethod]
        public void CheckAVS_D_X_MixF() //check F_D transition. It should be without crossfades
        {
            SetUp();
            model.Montage.Chunks.Mark(1000, new bool[] { false, true }, false);
            model.Montage.Chunks.Mark(2000, new bool[] { false, false }, false);
            model.Montage.Chunks.Mark(3000, new bool[] { true, true }, false);

            model.Montage.Chunks.Mark(4000, new bool[] { false, true }, false);

            model.Montage.Chunks.Mark(5000, new bool[] { false, true }, false);
            model.FormPreparedChunks();
            var nodes = serv.GetEpisodesNodes(model)[0] as AvsConcatList;
            Assert.AreEqual(typeof(AvsMix), nodes.Items[1].ChildNodes[0].GetType());
        }

        [TestMethod]
        public void CheckAVS_F_X_FMixF_X_D() //
        {
            SetUp();
            model.Montage.Chunks.Mark(1000, new bool[] { true, true }, false);
            model.Montage.Chunks.Mark(2000, new bool[] { false, false }, false);
            //fade shoulde be here
            model.Montage.Chunks.Mark(3000, new bool[] { true, true }, false); //this block should be mixed
            model.Montage.Chunks.Mark(4000, new bool[] { false, true }, false);
            model.Montage.Chunks.Mark(5000, new bool[] { true, true }, false);
            model.Montage.Chunks.Mark(6000, new bool[] { false, false }, false);
            //fade shouldn't be here
            model.Montage.Chunks.Mark(7000, new bool[] { false, true}, false);

            model.FormPreparedChunks();
            var nodes = serv.GetEpisodesNodes(model)[0] as AvsConcatList;

            Assert.AreEqual(typeof(AvsCrossFade), nodes.Items[0].GetType());
            Assert.AreEqual(typeof(AvsFadeOut), nodes.Items[1].GetType());
        }

        private void SetUp()
        {
            serv = new AssemblerService();
            model = new EditorModel(new DirectoryInfo("test"), new DirectoryInfo("test"), new DirectoryInfo("test"));
        }
    }
}

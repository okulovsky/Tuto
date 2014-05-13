using Montager;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using VideoLib;
using System.Collections.Generic;
using System.Linq;

namespace Montager.Tests
{


    [TestClass()]
    public class BatchCommandGenerator
    {
        [TestMethod()]
        public void Aggregation1()
        {
            var chunks = new List<Chunk> { new Chunk { Id = 1 }, new Chunk { Id = 2 } };
            var res = Montager.Aggregation2(chunks, "output").ToList();
            Assert.AreEqual(3, res.Count);
            Assert.IsInstanceOfType(res[0], typeof(ConcatCommand));
            {
                var c = (ConcatCommand)res[0];
                Assert.AreEqual(2, c.Files.Count);
                Assert.AreEqual("audio001.mp3", c.Files[0]);
                Assert.AreEqual("audio002.mp3", c.Files[1]);
                Assert.AreEqual("TotalAudio.mp3", c.Result);
                Assert.AreEqual(true, c.AudioOnly);
            }
            Assert.IsInstanceOfType(res[1], typeof(ConcatCommand));
            {
                var c = (ConcatCommand)res[1];
                Assert.AreEqual(2, c.Files.Count);
                Assert.AreEqual("video001.avi", c.Files[0]);
                Assert.AreEqual("video002.avi", c.Files[1]);
                Assert.AreEqual("TotalVideo.avi", c.Result);
                Assert.AreEqual(false, c.AudioOnly);
            }
            Assert.IsInstanceOfType(res[2], typeof(MixVideoAudioCommand));
            {
                var c = (MixVideoAudioCommand)res[2];
                Assert.AreEqual("TotalAudio.mp3", c.AudioInput);
                Assert.AreEqual("TotalVideo.avi", c.VideoInput);
                Assert.AreEqual("output", c.VideoOutput);
            }
        }


        [TestMethod()]
       public  void FaceTest2()
        {
            var chunk = new Chunk
            {
                IsFaceChunk=true,
                Id = 23,
                VideoSource = new ChunkSource { StartTime = 1000, File = "face", Duration = 1000 }
              
            };

            var cmds = Montager.Commands2(chunk).ToList();
            Assert.AreEqual(2, cmds.Count);
            Assert.IsInstanceOfType(cmds[0], typeof(ExtractFaceVideoCommand));
            {
                var c = (ExtractFaceVideoCommand)cmds[0];
                Assert.AreEqual("face", c.VideoInput);
                Assert.AreEqual("video023.avi", c.VideoOutput);
                Assert.AreEqual(1000, c.StartTime);
                Assert.AreEqual(1000, c.Duration);
            }
            Assert.IsInstanceOfType(cmds[1], typeof(ExtractAudioCommand));
            {
                var c = (ExtractAudioCommand)cmds[1];
                Assert.AreEqual("face", c.VideoInput);
                Assert.AreEqual("audio023.mp3", c.AudioOutput);
                Assert.AreEqual(1000, c.StartTime);
                Assert.AreEqual(1000, c.Duration);
            }
           

        }
        [TestMethod()]
        public void ScreenTest2()
        {
            var chunk = new Chunk
            {
                IsFaceChunk=false,
                Id = 23,
                VideoSource = new ChunkSource { StartTime = 1000, File = "screen", Duration = 1000 },
                AudioSource = new ChunkSource { StartTime = 2000, File = "face", Duration = 1000 }
            };
       
            var cmds = Montager.Commands2(chunk).ToList();
            Assert.AreEqual(2, cmds.Count);
            Assert.IsInstanceOfType(cmds[0], typeof(ExtractScreenVideoCommand));
            {
                var c = (ExtractScreenVideoCommand)cmds[0];
                Assert.AreEqual("screen", c.VideoInput);
                Assert.AreEqual("video023.avi", c.VideoOutput);
                Assert.AreEqual(1000, c.StartTime);
                Assert.AreEqual(1000, c.Duration);
            }
            Assert.IsInstanceOfType(cmds[1], typeof(ExtractAudioCommand));
            {
                var c = (ExtractAudioCommand)cmds[1];
                Assert.AreEqual("face", c.VideoInput);
                Assert.AreEqual("audio023.mp3", c.AudioOutput);
                Assert.AreEqual(2000, c.StartTime);
                Assert.AreEqual(1000, c.Duration);
            }
        }


        [TestMethod()]
        public void FaceTest()
        {
            var chunk = new Chunk { IsFaceChunk=true, Id = 23, VideoSource = new ChunkSource { StartTime = 1000, File = "face", Duration = 1000 } };
            var cmds = Montager.Commands1(chunk).ToList();
            Assert.AreEqual(1, cmds.Count);
            Assert.IsInstanceOfType(cmds[0], typeof(ExtractFaceVideoCommand));
            var c = (ExtractFaceVideoCommand)cmds[0];
            Assert.AreEqual("face", c.VideoInput);
            Assert.AreEqual("chunk023.avi", c.VideoOutput);
            Assert.AreEqual(1000, c.StartTime);
            Assert.AreEqual(1000, c.Duration);
        }
        

        [TestMethod()]
        public void ScreenTest()
        {
            var chunk = new Chunk
            {
                IsFaceChunk=false,
                Id = 23,
                VideoSource = new ChunkSource { StartTime = 1000, File = "screen", Duration = 1000 },
                AudioSource = new ChunkSource { StartTime = 2000, File = "face", Duration = 1000 }
            };
            var cmds = Montager.Commands1(chunk).ToList();
            Assert.AreEqual(3, cmds.Count);
            Assert.IsInstanceOfType(cmds[0], typeof(ExtractAudioCommand));
            {
                var c = (ExtractAudioCommand)cmds[0];
                Assert.AreEqual("face", c.VideoInput);
                Assert.AreEqual("audio023.mp3", c.AudioOutput);
                Assert.AreEqual(2000, c.StartTime);
                Assert.AreEqual(1000, c.Duration);
            }
            Assert.IsInstanceOfType(cmds[1], typeof(ExtractScreenVideoCommand));
            {
                var c = (ExtractScreenVideoCommand)cmds[1];
                Assert.AreEqual("screen", c.VideoInput);
                Assert.AreEqual("video023.avi", c.VideoOutput);
                Assert.AreEqual(1000, c.StartTime);
                Assert.AreEqual(1000, c.Duration);
            }
            Assert.IsInstanceOfType(cmds[2], typeof(MixVideoAudioCommand));
            {
                var c = (MixVideoAudioCommand)cmds[2];
                Assert.AreEqual("video023.avi", c.VideoInput);
                Assert.AreEqual("audio023.mp3", c.AudioInput);
                Assert.AreEqual("chunk023.avi", c.VideoOutput);
            }

        }
    }
}

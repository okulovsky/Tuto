using Montager;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using VideoLib;
using System.Collections.Generic;

namespace Montager.Tests
{


    [TestClass()]
    public class MontagerTest
    {
        static string faceFile="Face";
        static string screenFile="Screen";

        static MontageLog CreateMontageLog(int fileSync, params object[] data)
        {
            var result = new MontageLog();
            result.FaceFileSync = fileSync;
            for (int i = 0; i < data.Length; i += 2)
                result.Commands.Add(new MontageCommand { Id=i, Time = (int)data[i], Action = (MontageAction)data[i + 1] });
            return result;

        }

        static Action<Chunk,int> IsFace(int start, int dur=1000)
        {
            return (c,i) =>
                {
                    Assert.IsNotNull(c.VideoSource);
                    Assert.AreEqual(faceFile, c.VideoSource.File);
                    Assert.AreEqual(start, c.VideoSource.StartTime);
                    Assert.AreEqual(dur, c.VideoSource.Duration);
                    Assert.IsNull(c.AudioSource);
                    Assert.IsTrue(c.IsFaceChunk);
                };
        }

        static Action<Chunk, int> IsScreen(int vstart, int astart, int dur=1000)
        {
            return (c,i) =>
                {
                    Assert.IsNotNull(c.VideoSource);
                    Assert.AreEqual(screenFile, c.VideoSource.File);
                    Assert.AreEqual(vstart, c.VideoSource.StartTime);
                    Assert.AreEqual(dur, c.VideoSource.Duration);

                    Assert.IsNotNull(c.AudioSource);
                    Assert.AreEqual(faceFile, c.AudioSource.File);
                    Assert.AreEqual(astart, c.AudioSource.StartTime);
                    Assert.AreEqual(dur, c.AudioSource.Duration);

                    Assert.IsFalse(c.IsFaceChunk);
                };
        }

        static void Test(MontageLog actions, params Action<Chunk,int>[] checks)
        {
            var chunks = Montager.CreateChunks(actions, faceFile, screenFile);
            Assert.AreEqual(checks.Length, chunks.Count);
            for (int i = 0; i < chunks.Count; i++)
            {
               // Assert.AreEqual(i, chunks[i].Id); по идее, их ID надо проверять отдельно
                checks[i](chunks[i], i);
            }

        }


        [TestMethod()]
        public void SimpleCommit()
        {
            var commands = CreateMontageLog(2000,
                1000, MontageAction.StartFace,
                2000, MontageAction.StartScreen,
                3000, MontageAction.Commit,
                4000, MontageAction.Delete);

            Test(commands,
                IsFace(3000));
        }

        [TestMethod()]
        public void CommitSequence()
        {
            var commands = CreateMontageLog(2000,
                1000, MontageAction.StartFace,
                2000, MontageAction.StartScreen,
                3000, MontageAction.Commit,
                4000, MontageAction.Commit,
                5000, MontageAction.Delete,
                6000, MontageAction.Commit
                );

            Test(commands,
                IsFace(3000),
                IsFace(4000),
                IsFace(6000));

        }

        [TestMethod()]
        public void ScreenFace()
        {
            var commands = CreateMontageLog(2000
                , 1000, MontageAction.StartFace
                , 2000, MontageAction.StartScreen
                , 3000, MontageAction.Commit
                , 4000, MontageAction.Screen
                , 5000, MontageAction.Face
                );

            Test(commands
                , IsFace(3000)
                , IsFace(4000)
                , IsScreen(2000, 5000)
                );

        }

        [TestMethod()]
        public void ScreenFaceDelete()
        {
            var commands = CreateMontageLog(2000
                , 1000, MontageAction.StartFace
                , 2000, MontageAction.StartScreen
                , 3000, MontageAction.Screen
                , 4000, MontageAction.Delete
                , 5000, MontageAction.Commit
                , 6000, MontageAction.Face
                );

            Test(commands
                , IsFace(3000)
                , IsScreen(2000, 5000)
                , IsScreen(3000, 6000)
                );

        }
        
        
    }
}

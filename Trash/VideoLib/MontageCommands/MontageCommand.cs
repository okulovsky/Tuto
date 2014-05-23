using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VideoLib
{
    public enum MontageAction
    {
        StartFace,
        StartScreen,
        Face,
        Screen,
        Commit,
        Delete,
        CommitAndSplit
    }

    public class MontageCommand
    {
        public int Id;
        public int Time;
        public MontageAction Action;
    }

    public class MontageLog
    {
        public readonly List<MontageCommand> Commands = new List<MontageCommand>();
        public int FaceFileSync;
    }
}

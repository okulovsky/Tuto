using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VideoLib;

namespace Operator
{
    public static class Log
    {
        public static string FileName = "log.txt";
        static DateTime recordStartTime;
        static DateTime lastCommitTime;
        static TimeSpan goodSplitTime;
        static TimeSpan goodStartTime;
        static int Id;
        
        public static void Start()
        {
            recordStartTime = DateTime.Now;
            lastCommitTime = DateTime.Now;
            goodSplitTime = new TimeSpan();
            goodStartTime = new TimeSpan();
            Id = 0;
            MontageCommandIO.Clear(FileName);
        }

        public static void Commit(MontageAction action)
        {

            var now = DateTime.Now;
           
            var time = (int)(now - recordStartTime).TotalMilliseconds;
            var cmd = new MontageCommand
            {
                Action = action,
                Time = time,
                Id = Log.Id,
            };


            MontageCommandIO.AppendCommand(cmd, FileName);
            Id++;

            if (action == MontageAction.Commit)
            {
                goodSplitTime += now - lastCommitTime;
            }

            if (action == MontageAction.CommitAndSplit)
            {
                goodStartTime += goodSplitTime;
                goodSplitTime = new TimeSpan();
            }

            lastCommitTime = now;

        }

        public static TimeSpan TimeFromLastCommit { get { return DateTime.Now - lastCommitTime; } }

        public static TimeSpan TimeFromLastSplit { get { return goodSplitTime; } }

        public static TimeSpan TimeFromStart { get { return goodStartTime; } }

    }
}

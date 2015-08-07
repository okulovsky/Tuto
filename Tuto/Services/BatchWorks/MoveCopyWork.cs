using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuto.BatchWorks
{
    public class MoveCopyWork : BatchWork
    {
        private string from;
        private string to;
        private bool moveMode;

        public MoveCopyWork(string from, string to, bool move)
        {
            this.from = from;
            this.to = to;
            this.moveMode = move;
            Name = "Process file " + from;
        }

        public override void Work()
        {
            if (!moveMode)
                File.Copy(from, to);
            else File.Move(from, to);
            OnTaskFinished();
        }
    }
}

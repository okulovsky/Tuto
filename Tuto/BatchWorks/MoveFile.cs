using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Tuto.Model;
using System.Threading;

namespace Tuto.BatchWorks
{
    public class MoveFile: BatchWork
    {

        private FileInfo from { get; set; }
        private FileInfo to { get; set; }

        public MoveFile(FileInfo from, FileInfo to)
        {
            Name = string.Format("Move \"{0}\" to Output folder", from.FullName);
            this.from = from;
            this.to = to;
        }

        public override void Work()
        {
            if (File.Exists(to.FullName))
                to.Delete();
            File.Move(from.FullName, to.FullName);
        }
    }
}

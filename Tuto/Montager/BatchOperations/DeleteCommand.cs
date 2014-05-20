using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Montager
{
    public class DeleteCommand : BatchCommand
    {
        public string FileName;
        public override string Caption
        {
            get { return "Удаление файла " + FileName; }
        }
        public override void WriteToBatch(BatchCommandContext context)
        {
            File.Delete(FileName);
        }
    }
}

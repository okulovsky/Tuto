using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Editor;

namespace ModelCreator
{
    class Program
    {
        static void Main(string[] args)
        {
            var path=ObsoleteModelIO.DebugSubdir("debug\\20");
            Directory.CreateDirectory(path);
            File.Delete(path + "\\montage.v3");
            var model = ObsoleteModelIO.Load(path);
            for (int i = 0; i < 1000; i++)
            {
                model.SetChunkMode(i*3000, (i%2==0)?Mode.Face:Mode.Screen,false);
            }
            for (int i = 0; i < 1000; i++)
            {
                model.Montage.Intervals.Add(new Interval { StartTime = i * 3000, EndTime = i * 3000 + 500, HasVoice = false });
                model.Montage.Intervals.Add(new Interval { StartTime = i * 3000+500, EndTime = i * 3000+ 3000, HasVoice = true });
            }
            ObsoleteModelIO.Save(model);
        }
    }
}

using Editor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuto.Model;

namespace ModelCreator
{
    class Program
    {
        static void Main(string[] args)
        {
            var path=EditorModelIO.SubstituteDebugDirectories("debug\\10");
            Directory.CreateDirectory(path);
            File.Delete(path + "\\local.tuto");
            var model = EditorModelIO.Load(path);
            model.Save();
        }
    }
}

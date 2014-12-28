﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Editor;
using Tuto.Model;

namespace Updater
{
    class Program
    {
        static void Main(string[] args)
        {
            var srcDirectory = new DirectoryInfo(args[0]);
            var dstDirectory = srcDirectory.Parent.CreateSubdirectory(srcDirectory.Name + "-converted");
            var input = dstDirectory.CreateSubdirectory("Input");
            var output = dstDirectory.CreateSubdirectory("Output");

            var global = new GlobalData();
            global.AfterLoad(dstDirectory);
            EditorModelIO.Save(global);


            var copying = "";

            foreach (var e in srcDirectory.GetDirectories())
            {
                var model = new EditorModel(e, srcDirectory, srcDirectory);
                ObsoleteModelIO.LoadAndConvert(model);
                var modelDst = input.CreateSubdirectory(e.Name);
                model.HackLocations(srcDirectory, modelDst);
                model.Save();
                copying += string.Format("copy {0}\\{2} {1}\\{2}\n", e.FullName, modelDst.FullName, "face.mp4");
                copying += string.Format("copy {0}\\{2} {1}\\{2}\n", e.FullName, modelDst.FullName, "desktop.avi");
                var files = e.GetFiles();
                foreach (var f in files.Where(z => z.Name.StartsWith("AIML") && z.Extension == ".avi"))
                    copying += string.Format("copy {0}\\{2} {1}\\{2}", e.FullName, output.FullName, f.Name);
            }

            File.WriteAllText(dstDirectory + "\\copy.bat", copying);
        }
    }
}

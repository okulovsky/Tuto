using System;
using System.IO;
using System.Linq;
using Tuto.Model;
using Tuto.TutoServices.Montager;

namespace Tuto.TutoServices
{
    public class RepairService : Service
    {
        public override string Name
        {
            get { return Services.Repair.ToString(); }
        }

        public override string Description
        {
            get { return DescriptionString; }
        }

        public override string Help
        {
            get { return HelpString; }
        }

        void ProcessDesktop(FileInfo brokenFile, FileInfo outputFile, bool print = false)
        {
            new RepairCommand { VideoInput = brokenFile, VideoOutput = outputFile }.Execute(print);
        }

        void ProcessFace(FileInfo brokenFile, FileInfo outputFile, bool print = false)
        {
            var temp = new FileInfo(Path.Combine(outputFile.Directory.FullName, "temp.avi"));
            new RepairCommand { VideoInput = brokenFile, VideoOutput = temp }.Execute(print);
            File.Move(temp.FullName, outputFile.FullName);
        }


        public void DoWork(FileInfo file, bool face, bool print=false)
        {
            FileInfo brokenFile=null;
            for (int i=0;;i++)
            {
                var name=Path.GetFileNameWithoutExtension(file.Name)+"_"+i.ToString()+file.Extension;
                name = Path.Combine(file.Directory.FullName, name);
                brokenFile=new FileInfo(name);
                if (brokenFile.Exists) continue;
                File.Move(file.FullName,brokenFile.FullName);
                break;
            }
            FileInfo outputFile=file;
            if (!face) ProcessDesktop(brokenFile, outputFile, print);
            else ProcessFace(brokenFile, outputFile, print);
        }

        public override void DoWork(string[] args)
        {
            if (args.Length < 4)
                throw (new ArgumentException(String.Format("Insufficient args")));
            var folder = args[1];

            bool face = args[2] == "face";
            if (args[2] != "face" && args[2] != "desktop") throw new Exception("The second argument must be 'face' or 'desktop'");

            ExecMode mode;
            if (!Enum.TryParse(args[2], true, out mode))
                throw (new ArgumentException(String.Format("Unknown mode: {0}", args[2])));
            var print = mode == ExecMode.Print;

            var model = EditorModelIO.Load(folder);
            if (face)
                DoWork(model.Locations.FaceVideo, face, print);
            else
                DoWork(model.Locations.DesktopVideo, face, print);
        }

      
        const string DescriptionString =
            @"Recodes video to fix broken files.";
        const string HelpString =
            @"<folder> <mode>

folder:         directory containing desktop and face videos
face|desktop:   which file should be repaired 
mode:           run or print. Execute commands or write them to stdout

The original file is stored with suffix, the repaired - under the name of original.";
    }
}
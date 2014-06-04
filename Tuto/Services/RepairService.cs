using System;
using System.IO;
using System.Linq;
using Tuto.Model;
using Tuto.Services.Montager;

namespace Tuto.Services
{
    class RepairService : Service
    {
        public override string Name
        {
            get { return "repair"; }
        }

        public override string Description
        {
            get { return DescriptionString; }
        }

        public override string Help
        {
            get { return string.Format(HelpString, FilenameSuffix); }
        }

        public void DoWork(EditorModel model, bool print)
        {
            var videos = new[] {model.Locations.FaceVideo, model.Locations.DesktopVideo};
            foreach (var videoFile in videos)
            {
                var repairedFilename = Path.GetFileNameWithoutExtension(videoFile.Name) + FilenameSuffix;
                var repairedVideo = model.Locations.Make(model.VideoFolder, repairedFilename);
                if (repairedVideo.Exists)
                    repairedVideo.Delete();
                var command = new RepairCommand {VideoInput = videoFile, VideoOutput = repairedVideo};
                command.Execute(print);
            }
        }

        public override void DoWork(string[] args)
        {
            if (args.Length < 3)
                throw (new ArgumentException(String.Format("Insufficient args")));
            var folder = args[1];
            ExecMode mode;
            if (!Enum.TryParse(args[2], true, out mode))
                throw (new ArgumentException(String.Format("Unknown mode: {0}", args[2])));
            var print = mode == ExecMode.Print;

            var model = EditorModelIO.Load(folder);
            DoWork(model, print);
        }

        static string FilenameSuffix {get { return "_repaired.avi"; }}

        const string DescriptionString =
            @"Recodes video to fix broken files.";
        const string HelpString =
            @"<folder> <mode>

folder: directory containing desktop and face videos
mode: run or print. Execute commands or write them to stdout

Repaired video is stored as ""<filename>{0}"".";
    }
}
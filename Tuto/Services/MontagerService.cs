using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Editor;
using Tuto.Model;
using System.Threading;
using System.IO;

namespace Tuto.TutoServices
{
    public class MontagerService : Service
    {

        public override string Name
        {
            get { return Services.Montager.ToString(); } 
        }

        public override string Description
        {
            get { return DescriptionString; }
        }

        public override string Help
        {
            get { return HelpString; }
        }

        public void DoWork(EditorModel model, bool print)
        {
            if (!model.ChunkFolder.Exists)
            {
                model.ChunkFolder.Delete(true);
                model.ChunkFolder.Create();
            }
            Thread.Sleep(100); //без этого почему-то вылетают ошибки
            if (File.Exists(model.Locations.FaceVideo.FullName) && !File.Exists(model.Locations.ConvertedFaceVideo.FullName))
                Shell.FFMPEG(print, @"-i ""{0}"" -vf scale=1280:720 -q:v 0 -acodec libmp3lame -ar 44100 -ab 32k ""{1}""",
                    model.Locations.FaceVideo.FullName, model.Locations.ConvertedFaceVideo.FullName);

            if (File.Exists(model.Locations.DesktopVideo.FullName) && !File.Exists(model.Locations.ConvertedDesktopVideo.FullName))
                Shell.FFMPEG(print, @"-i ""{0}"" -vf scale=1280:720 -q:v 0 -an ""{1}""",
                                model.Locations.DesktopVideo.FullName, model.Locations.ConvertedDesktopVideo.FullName);
        }

        public override void DoWork(string[] args)
        {
            if(args.Length < 3)
                throw (new ArgumentException(String.Format("Insufficient args")));
            var folder = args[1];
            ExecMode mode;
            if (!Enum.TryParse(args[2], true, out mode))
                throw (new ArgumentException(String.Format("Unknown mode: {0}", args[2])));
            var print = mode == ExecMode.Print;

            var model = EditorModelIO.Load(folder);
            model.FormPreparedChunks();            
            DoWork(model, print);
            model.Montage.Montaged = true;
            EditorModelIO.Save(model);
        }
        const string DescriptionString =
@"Muxes audio and video from different sources for assembling.";
        const string HelpString =
@"<folder> <mode>

folder: directory containing video
mode: run or print. Execute commands or write them to stdout";
    }
}

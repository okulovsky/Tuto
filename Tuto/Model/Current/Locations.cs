using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuto.Model
{
    public class Locations
    {
        readonly EditorModel model;
        internal Locations(EditorModel model) { this.model = model; }

        internal FileInfo Make(DirectoryInfo info, string fname)
        {
            return new FileInfo(Path.Combine(info.FullName, fname));
        }

        public FileInfo PraatExecutable { get { return Make(model.ProgramFolder, "praatcon.exe"); } }
        public FileInfo PraatScriptSource { get { return Make(model.ProgramFolder, "split_pauses.praat"); } }
        public FileInfo AvsLibrary { get { return Make(model.ProgramFolder, "library.avs"); } }
        public FileInfo AutoLevelsLibrary { get { return Make(model.ProgramFolder, "autolevels_0.6_20110109.dll"); } }
        public FileInfo TutoExecutable { get { return Make(model.ProgramFolder, "Tuto.exe"); } }
        
        public FileInfo FaceVideo { get { return Make(model.VideoFolder, "face.mp4"); } }
        public FileInfo DesktopVideo { get { return Make(model.VideoFolder, "desktop.avi"); } }
        public FileInfo PraatVoice { get { return Make(model.VideoFolder, "voice.mp3"); } }
        public FileInfo LocalFilePath { get { return Make(model.VideoFolder, LocalFileName); } }

        public DirectoryInfo OutputDirectory { get { return new DirectoryInfo(Path.Combine(model.RootFolder.FullName, "Output")); } }
        
        public FileInfo PraatOutput { get { return Make(model.VideoFolder, "praat.output"); } }
        public FileInfo AvsTempFile { get { return Make(model.ChunkFolder, String.Format("script_{0}.avs", Guid.NewGuid())); } }

        public FileInfo IntroImage { get { return Make(model.VideoFolder, "intro.png"); } }
        public FileInfo WatermarkImage { get { return Make(model.VideoFolder, "watermark.png"); } }

        public const string LocalFileName = "local.tuto";
        public const string GlobalFileName = "project.tuto";
    }
}

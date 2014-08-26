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
        public FileInfo AutoLevelsLibrary { get { return Make(model.ProgramFolder, "autolevels.dll"); } }
        public FileInfo VSFilterLibrary { get { return new FileInfo(@"C:\Program Files\VSFilter\VSFilter.dll"); } }
        
        
        public FileInfo TutoExecutable { get { return Make(model.ProgramFolder, "Tuto.exe"); } }

        public FileInfo FaceVideo { get { return Make(model.VideoFolder, FaceVideoFileName); } }
        public FileInfo DesktopVideo { get { return Make(model.VideoFolder, DesktopVideoFileName ); } }
        
        public FileInfo ConvertedFaceVideo { get { return Make(model.ChunkFolder, "face-converted.avi"); } }
        public FileInfo ConvertedDesktopVideo { get { return Make(model.ChunkFolder, "desktop-converted.avi"); } }
        
        public FileInfo PraatVoice { get { return Make(model.VideoFolder, "voice.mp3"); } }
        public FileInfo LocalFilePath { get { return Make(model.VideoFolder, LocalFileName); } }

        public DirectoryInfo OutputDirectory { get { return new DirectoryInfo(Path.Combine(model.RootFolder.FullName, Locations.OutputFolderName)); } }
        
        public FileInfo PraatOutput { get { return Make(model.VideoFolder, "praat.output"); } }

       

        public FileInfo IntroImage { get { return Make(model.VideoFolder, "intro.png"); } }
        public FileInfo WatermarkImage { get { return Make(model.VideoFolder, "watermark.png"); } }

        public const string LocalFileName = "local.tuto";
        public const string GlobalFileName = "project.tuto";
        public const string FaceVideoFileName = "face.mp4";
        public const string DesktopVideoFileName = "desktop.avi";
        public const string TemporalFolderName = "chunks";
        public const string OutputFolderName = "Output";

        public FileInfo GetOutputFile(int episodeNumber)
        {
            if (!model.Locations.OutputDirectory.Exists)
                model.Locations.OutputDirectory.Create();
            var file = new FileInfo(
            Path.Combine(
                    model.Locations.OutputDirectory.FullName,
                    string.Format("{0}-{1} {2}.avi",
                        model.VideoFolder.Name,
                        episodeNumber,
                        model.Montage.Information.Episodes[episodeNumber].Name)));
            return file;
        }

        public FileInfo GetAvsStriptFile(int episodeNumber)
        {
            return new FileInfo(
                Path.Combine(
                    model.ChunkFolder.FullName,
                    string.Format("script_{0}.avs", episodeNumber)));
        }

        public FileInfo GetSrtFile(int episodeNumber)
        {
            return new FileInfo(
                Path.Combine(
                    model.ChunkFolder.FullName,
                    string.Format("subtitles_{0}.srt", episodeNumber)));
        }


    }
}

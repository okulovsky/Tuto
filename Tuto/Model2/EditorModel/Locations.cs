using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuto.Model
{
    public class NewLocations
    {
        readonly NewEditorModel model;
		internal NewLocations(NewEditorModel model) { this.model = model; }

        internal FileInfo Make(DirectoryInfo info, string fname)
        {
            return new FileInfo(Path.Combine(info.FullName, fname));
        }

        internal FileInfo GetSuffixedName(FileInfo source, string suffix)
        {
            var newPath = source.FullName.Split('\\');
            var nameAndExt = source.Name.Split('.');
            nameAndExt[0] = nameAndExt[0] + suffix;
            newPath[newPath.Length - 1] = string.Join(".", nameAndExt);
            return new FileInfo(string.Join("\\", newPath));
        }

        internal FileInfo GetThumbName(FileInfo source)
        {
            return GetSuffixedName(source, "-thumb");
        }

        
		public FileInfo ClearedSound { get { return Make(model.Locations.FaceVideo.Directory, "cleaned.mp3"); } }
        
        
        

        public FileInfo FaceVideo { get { return Make(model.RawLocation, FaceVideoFileName); } }
        public FileInfo DesktopVideo { get { return Make(model.RawLocation, DesktopVideoFileName ); } }

        public FileInfo FaceVideoThumb { get { return GetThumbName(FaceVideo); } }
        public FileInfo DesktopVideoThumb { get { return GetThumbName(DesktopVideo); } }
        
        public FileInfo ConvertedFaceVideo { get { return Make(model.TempFolder, "face-converted.avi"); } }
        public FileInfo ConvertedDesktopVideo { get { return Make(model.TempFolder, "desktop-converted.avi"); } }
        
        public FileInfo PraatVoice { get { return Make(model.RawLocation, "voice.mp3"); } }
        public FileInfo LocalFilePath { get { return Make(model.RawLocation, LocalFileName); } }

 
        public DirectoryInfo PatchesDirectory
        {
            get
            {
                var relative = model.Global.Locations.RelativeTo(model.RawLocation.FullName, model.Global.Locations.InputFolder.FullName);
                var name = Path.Combine(model.Global.Locations.PatchesFolder.FullName, relative);
                return new DirectoryInfo(name);
            }
        }

        public FileInfo PraatOutput { get { return Make(model.VideoFolder, "praat.output"); } }

       

        public FileInfo IntroImage { get { return Make(model.VideoFolder, "intro.png"); } }
        public FileInfo WatermarkImage { get { return Make(model.VideoFolder, "watermark.png"); } }

        public const string LocalFileName = "local.tuto";
        public const string GlobalFileName = "project.tuto";
        public const string PublishingFileName = "publishing.tuto";
        public const string FaceVideoFileName = "face.mp4";
        public const string DesktopVideoFileName = "desktop.avi";
        public const string TemporalFolderName = "chunks";
        public const string OutputFolderName = "Output";
        public const string InputFolderName = "Input";
        public const string AllTemporaryFilesFolder = "Temp";
        public const string ConvertedPatchFilesFolder = "Patches";

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
                    model.TempFolder.FullName,
                    string.Format("script_{0}.avs", episodeNumber)));
        }

        public FileInfo GetSrtFile(int episodeNumber)
        {
            return new FileInfo(
                Path.Combine(
                    model.TempFolder.FullName,
                    string.Format("subtitles_{0}.srt", episodeNumber)));
        }


    }
}

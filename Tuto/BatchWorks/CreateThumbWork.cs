using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Tuto.Model;
using System.Windows.Controls;
using System.Threading;

namespace Tuto.BatchWorks
{
    public class CreateThumbWork : BatchWork
    {
        public CreateThumbWork(FileInfo source, EditorModel model, bool forced)
        {
            Name = "Thumb Video: " + source;
            this.Source = source;
            this.Model = model;
            Forced = forced;
            BeforeWorks.Add(new CreateCleanSoundWork(model.Locations.FaceVideo, model, false));
        }

        public FileInfo Source;
        private FileInfo tempFile;
        public FileInfo ThumbName;

        public override void Work()
        {
            var codec = "-vcodec libxvid";
            var newPath = Source.FullName.Split('\\');
            var nameAndExt = Source.Name.Split('.');
            nameAndExt[0] = nameAndExt[0] + "-thumb";
            newPath[newPath.Length - 1] = string.Join(".", nameAndExt);
            ThumbName = new FileInfo(string.Join("\\", newPath));
            tempFile = GetTempFile(Source);

            var argsWithoutCleaning = string.Format(@"-i ""{0}"" -r 25 -q:v 15 {2} -acodec libmp3lame -ar 44100 -ab 32k ""{1}"" -y",
                    Source.FullName, tempFile.FullName, codec);

            var argsWithCleaning = string.Format(@"-i ""{0}"" -i ""{3}"" -map 0:0 -map 1 -shortest -r 25 -q:v 15 {2} -acodec libmp3lame -ar 44100 -ab 32k  ""{1}"" -y",
                    Source.FullName, tempFile.FullName, codec, Model.Locations.ClearedSound.FullName);

            var args = Model.Videotheque.Data.WorkSettings.AudioCleanSettings.CurrentOption != Options.Skip ? argsWithCleaning : argsWithoutCleaning;
            var fullPath = Model.Videotheque.Locations.FFmpegExecutable;
            RunProcess(args, fullPath.FullName);
            Thread.Sleep(500);
            if (File.Exists(ThumbName.FullName))
                File.Delete(ThumbName.FullName);
            File.Move(tempFile.FullName, ThumbName.FullName);
            OnTaskFinished();
        }

        public override bool Finished()
        {
            return Model.Locations.GetThumbName(Source).Exists;
        }

        public override void Clean()
        {
            FinishProcess();
            TryToDelete(tempFile.FullName);
        }
    }
}

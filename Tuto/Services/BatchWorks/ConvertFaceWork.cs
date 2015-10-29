using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Tuto.Model;
using System.Threading;

namespace Tuto.BatchWorks
{
    public class ConvertFaceWork : ConvertVideoWork
    {
        public ConvertFaceWork(EditorModel model)
        {
            Model = model;
            Name = "Converting Face: " + Model.Locations.FaceVideo.FullName;
            source = Model.Locations.FaceVideo;
            nonConvertedFile = new FileInfo(Path.Combine(Model.Locations.TemporalDirectory.FullName, Path.ChangeExtension(Model.Locations.FaceVideo.Name, ".avi")));
            tempFile = GetTempFile(nonConvertedFile);
            convertedFile = GetTempFile(nonConvertedFile, "-converted");
        }

        public override bool Finished()
        {
            return Model.Locations.ConvertedFaceVideo.Exists;
        }

        public override void Clean()
        {
            FinishProcess();
            TryToDelete(tempFile.FullName);
        }
    }
}

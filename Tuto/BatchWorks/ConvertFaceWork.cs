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

        public ConvertFaceWork(EditorModel model, bool forced)
        {
            Model = model;
            Name = "Converting Face video: " + Model.RawLocation.Name;
            source = Model.Locations.FaceVideo;
            Forced = forced;
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

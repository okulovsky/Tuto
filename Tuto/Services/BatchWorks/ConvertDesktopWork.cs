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
    public class ConvertDesktopWork : ConvertVideoWork
    {
        public ConvertDesktopWork(EditorModel model)
        {
            Model = model;
            Name = "Converting Desktop: " + Model.Locations.DesktopVideo.FullName;
            source = Model.Locations.DesktopVideo;
        }

        public override bool Finished()
        {
            return Model.Locations.ConvertedDesktopVideo.Exists;
        }

        public override void Clean()
        {
            FinishProcess();
            TryToDelete(tempFile.FullName);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Tuto.Model;
using Tuto.TutoServices;
using Tuto.TutoServices.Assembler;

namespace Tuto.BatchWorks
{
    public class UploadVideoWork : CompositeWork
    {
        public UploadVideoWork (EditorModel model, bool forced)
        {
            Model = model;
            Name = "Upload: " + model.RawLocation.Name;

            for (var episodeNumber = 0; episodeNumber < model.Montage.Information.Episodes.Count; episodeNumber++)
            {
                if (Model.Montage.Information.Episodes[episodeNumber].OutputType == OutputTypes.None)
                    continue;                
                Tasks.Add(new YoutubeWork(model,episodeNumber, forced));
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Tuto.Model;
using System.Threading;
using Tuto.Publishing;

namespace Tuto.BatchWorks
{
    public class YoutubeWork : BatchWork
    {
        EpisodeBindingInfo info;
        List<EditorModel> models;
        public YoutubeWork(EpisodeBindingInfo info, List<EditorModel> models)
        {
            this.info = info;
            this.models = models;
            Name = "Uploading: " + info.Title;
        }


        public override void Work()
        {
            var w = new UploadVideo(info, models);
            w.Proceed();
        }

        public override void Clean()
        {
            if (Process != null && !Process.HasExited)
                Process.Kill();
        }
    }
}

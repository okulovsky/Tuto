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
        public YoutubeWork(EpisodeBindingInfo info)
        {
            this.info = info;
            Name = "Uploading: " + info.EpisodeInfo.Name;
        }


        public override void Work()
        {
            var w = new UploadVideo(info);
            w.Proceed();
        }

        public override void Clean()
        {
            if (Process != null && !Process.HasExited)
                Process.Kill();
        }
    }
}

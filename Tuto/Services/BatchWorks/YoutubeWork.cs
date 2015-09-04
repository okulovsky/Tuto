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
        FinishedVideo info;
        public YoutubeWork(FinishedVideo info)
        {
            this.info = info;
            Name = "Uploading: " + info.Name;
        }


        public override void Work()
        {
            var w = new UploadVideo(info);
            w.Uploaded += (s, a) => { OnTaskFinished(); };
            w.Proceed();
        }

        
        public override void Clean()
        {
            if (Process != null && !Process.HasExited)
                Process.Kill();
        }
    }
}

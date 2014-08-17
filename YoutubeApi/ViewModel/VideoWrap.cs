using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuto.Model;
using YoutubeApi.Model;

namespace YoutubeApi.ViewModel
{
    public enum Status
    {
        MatchedOld,
        MatchedNew,
        NotFoundAtYoutube,
        NotExpectedAtYoutube,
        DeletedFromYoutube,
        DeletedFromTuto,
        DeletedFromBoth
    }

    public class VideoWrap : Wrap
    {
        public FinishedVideo Finished { get; set; }
        public PublishedVideo Published { get; set; }
        public ClipData ClipData { get; set; }
        public List<Topic> TopicsStack { get; private set; }
        public Status Status { get; private set; }

        public VideoWrap(FinishedVideo fin, PublishedVideo pub, ClipData cl, Status status)
        {
            TopicsStack = new List<Topic>();
            Finished = fin;
            Published = pub;
            ClipData = cl;
            Status = status;
        }
    }
}

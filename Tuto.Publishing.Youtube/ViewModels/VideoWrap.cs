using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuto.Model;

namespace Tuto.Publishing.Youtube
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
        public Status Status { get; private set; }

        public string VideoURLFull { get { if (ClipData == null) return ""; return ClipData.VideoURLFull; } }
        public string VideoURLShort { get { if (ClipData == null) return ""; return ClipData.Id; } }
        public string YoutubeName { get { if (ClipData == null) return ""; return ClipData.Name; } }

        public VideoWrap(FinishedVideo fin, PublishedVideo pub, ClipData cl, Status status)
        {
            Finished = fin;
            Published = pub;
            ClipData = cl;
            Status = status;
        }

        public IEnumerable<TopicWrap> TopicsFromRoot
        {
            get
            {
                return PathFromRoot.Where(z => !z.Root).OfType<TopicWrap>();
            }
        }



    }
}

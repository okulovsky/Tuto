using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuto.Model;
using Tuto.Publishing.Youtube.Model;

namespace Tuto.Publishing.Youtube.ViewModel
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

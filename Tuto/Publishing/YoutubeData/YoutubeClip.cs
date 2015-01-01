using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuto.Publishing
{
    public class YoutubeClip
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string VideoURLFull { get { return "http://youtube.com/watch?v=" + Id; } }
        public string GDataURL { get { return "http://gdata.youtube.com/feeds/api/videos/" + Id; } }
        public override string ToString()
        {
            return Name;
        }
    }

    public class YoutubeClipMatcher<TItem> : Matcher<TItem, YoutubeClip>
        where TItem : IItem
    {

        static YoutubeClip BestMatch(TItem item, List<YoutubeClip> clips)
        {
            return NameMatchAlgorithm.FindBest(item.Caption, clips, z => z.Name);
        }

        public YoutubeClipMatcher(IEnumerable<YoutubeClip> clips)
            : base(
                clips,
                BestMatch,
                (a, b) => a.Id == b.Id)
        { }
    }
}

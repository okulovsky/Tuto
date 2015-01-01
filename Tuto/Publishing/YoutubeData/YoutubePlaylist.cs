using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Tuto.Publishing
{
    [DataContract]
    public class YoutubePlaylist
    {
        [DataMember]
        public string PlaylistId { get; set; }
        [DataMember]
        public string PlaylistTitle { get; set; }
    }


    public class YoutubePlaylistMatcher<TItem> : Matcher<TItem, YoutubePlaylist>
        where TItem : IItem
    {

        static YoutubePlaylist BestMatch(TItem item, List<YoutubePlaylist> clips)
        {
            return NameMatchAlgorithm.FindBest(item.Caption, clips, z => z.PlaylistTitle);
        }

        public YoutubePlaylistMatcher(IEnumerable<YoutubePlaylist> clips)
            : base(
                clips,
                BestMatch,
                (a, b) => a.PlaylistId == b.PlaylistId)
        { }
    }
}

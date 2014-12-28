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
        public Guid TopicGuid { get; set; }
        [DataMember]
        public string PlaylistId { get; set; }
    }

    public interface IYoutubePlaylistItem : IItem
    {
        YoutubePlaylist YoutubePlaylist { get; set; }
    }
}
